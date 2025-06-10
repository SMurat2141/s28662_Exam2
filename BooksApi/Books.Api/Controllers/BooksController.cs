using Microsoft.AspNetCore.Mvc;
using Books.Domain.Interfaces;
using Books.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Books.Api.Models.DTOs;

namespace Books.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BooksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks(
            [FromQuery] DateTime? fromReleaseDate,
            [FromQuery] DateTime? toReleaseDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var books = await _unitOfWork.Books.GetAllAsync();

            if (fromReleaseDate.HasValue)
                books = books.Where(b => b.ReleaseDate >= fromReleaseDate.Value);
            if (toReleaseDate.HasValue)
                books = books.Where(b => b.ReleaseDate <= toReleaseDate.Value);

            var ordered = books
                .OrderByDescending(b => b.ReleaseDate)
                .ThenBy(b => b.PublishingHouse.Name);

            var paged = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = paged.Select(b => new BookResponse
            {
                Id = b.Id,
                Name = b.Name,
                ReleaseDate = b.ReleaseDate,
                PublishingHouse = new PublishingHouseDto
                {
                    Id = b.PublishingHouse.Id,
                    Name = b.PublishingHouse.Name,
                    Country = b.PublishingHouse.Country,
                    City = b.PublishingHouse.City
                },
                Authors = b.BookAuthors
                    .Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}"),
                Genres = b.BookGenres
                    .Select(bg => bg.Genre.Name)
            });

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookResponse>> GetBook(int id)
        {
            var book = await _unitOfWork.Books
                .FindAsync(b => b.Id == id);
            var entity = book.FirstOrDefault();
            if (entity == null) return NotFound();

            var response = new BookResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                ReleaseDate = entity.ReleaseDate,
                PublishingHouse = new PublishingHouseDto
                {
                    Id = entity.PublishingHouse.Id,
                    Name = entity.PublishingHouse.Name,
                    Country = entity.PublishingHouse.Country,
                    City = entity.PublishingHouse.City
                },
                Authors = entity.BookAuthors
                    .Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}"),
                Genres = entity.BookGenres
                    .Select(bg => bg.Genre.Name)
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<BookResponse>> CreateBook(BookCreateRequest request)
        {
            if (request.AuthorIds.Count == 0 || request.GenreIds.Count == 0)
                return BadRequest("At least one author and one genre are required.");

            PublishingHouse? publishingHouse = null;

            if (request.PublishingHouse.Id != 0)
            {
                publishingHouse = await _unitOfWork.PublishingHouses.GetByIdAsync(request.PublishingHouse.Id);
            }

            if (publishingHouse == null)
            {
                publishingHouse = new PublishingHouse
                {
                    Name = request.PublishingHouse.Name,
                    Country = request.PublishingHouse.Country,
                    City = request.PublishingHouse.City
                };
                await _unitOfWork.PublishingHouses.AddAsync(publishingHouse);
                await _unitOfWork.SaveChangesAsync();
            }

            var missingAuthors = new List<int>();
            foreach (var authorId in request.AuthorIds)
            {
                var author = await _unitOfWork.Authors.GetByIdAsync(authorId);
                if (author == null) missingAuthors.Add(authorId);
            }
            if (missingAuthors.Any())
                return BadRequest($"Authors not found: {string.Join(',', missingAuthors)}");

            var missingGenres = new List<int>();
            foreach (var genreId in request.GenreIds)
            {
                var genre = await _unitOfWork.Genres.GetByIdAsync(genreId);
                if (genre == null) missingGenres.Add(genreId);
            }
            if (missingGenres.Any())
                return BadRequest($"Genres not found: {string.Join(',', missingGenres)}");

            var book = new Book
            {
                Name = request.Name,
                ReleaseDate = request.ReleaseDate,
                PublishingHouseId = publishingHouse.Id
            };

            foreach (var aid in request.AuthorIds.Distinct())
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = aid });
            }
            foreach (var gid in request.GenreIds.Distinct())
            {
                book.BookGenres.Add(new BookGenre { GenreId = gid });
            }

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, new { book.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, BookUpdateRequest request)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return NotFound();

            book.Name = request.Name;
            book.ReleaseDate = request.ReleaseDate;
            book.PublishingHouseId = request.PublishingHouseId;

            // Update relationships
            book.BookAuthors.Clear();
            foreach (var aid in request.AuthorIds.Distinct())
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = aid, BookId = id });
            }

            book.BookGenres.Clear();
            foreach (var gid in request.GenreIds.Distinct())
            {
                book.BookGenres.Add(new BookGenre { GenreId = gid, BookId = id });
            }

            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return NotFound();

            _unitOfWork.Books.Remove(book);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
