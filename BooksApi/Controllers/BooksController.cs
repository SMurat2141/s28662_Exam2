using BooksApi.Data;
using BooksApi.Models.DTOs;
using BooksApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns books filtered by optional release date range.
        /// </summary>
        /// <param name="fromReleaseDate">Inclusive start date</param>
        /// <param name="toReleaseDate">Inclusive end date</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks(
            [FromQuery] DateTime? fromReleaseDate,
            [FromQuery] DateTime? toReleaseDate)
        {
            var query = _context.Books
                .Include(b => b.PublishingHouse)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres).ThenInclude(bg => bg.Genre)
                .AsQueryable();

            if (fromReleaseDate.HasValue)
                query = query.Where(b => b.ReleaseDate >= fromReleaseDate.Value);

            if (toReleaseDate.HasValue)
                query = query.Where(b => b.ReleaseDate <= toReleaseDate.Value);

            var data = await query
                .OrderByDescending(b => b.ReleaseDate)
                .ThenBy(b => b.PublishingHouse.Name)
                .Select(b => new BookResponse
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
                        .Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}")
                        .ToList(),
                    Genres = b.BookGenres
                        .Select(bg => bg.Genre.Name)
                        .ToList()
                })
                .ToListAsync();

            return Ok(data);
        }

        /// <summary>
        /// Creates a new book. Adds a publishing house if it does not exist.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookResponse>> CreateBook([FromBody] BookCreateRequest request)
        {
            if (request.AuthorIds.Count == 0)
                return BadRequest("At least one author ID is required.");

            if (request.GenreIds.Count == 0)
                return BadRequest("At least one genre ID is required.");

            PublishingHouse? publishingHouse = null;

            if (request.PublishingHouse.Id != 0)
            {
                publishingHouse = await _context.PublishingHouses
                    .FirstOrDefaultAsync(ph => ph.Id == request.PublishingHouse.Id);
            }

            if (publishingHouse == null)
            {
                publishingHouse = new PublishingHouse
                {
                    Name = request.PublishingHouse.Name,
                    Country = request.PublishingHouse.Country,
                    City = request.PublishingHouse.City
                };
                _context.PublishingHouses.Add(publishingHouse);
                await _context.SaveChangesAsync();
            }

            var missingAuthor = await _context.Authors
                .Where(a => request.AuthorIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            if (missingAuthor.Count != request.AuthorIds.Count)
                return BadRequest("One or more authors not found.");

            var missingGenres = await _context.Genres
                .Where(g => request.GenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            if (missingGenres.Count != request.GenreIds.Count)
                return BadRequest("One or more genres not found.");

            var book = new Book
            {
                Name = request.Name,
                ReleaseDate = request.ReleaseDate,
                PublishingHouseId = publishingHouse.Id
            };

            foreach (var authorId in request.AuthorIds.Distinct())
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
            }

            foreach (var genreId in request.GenreIds.Distinct())
            {
                book.BookGenres.Add(new BookGenre { GenreId = genreId });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var response = new BookResponse
            {
                Id = book.Id,
                Name = book.Name,
                ReleaseDate = book.ReleaseDate,
                PublishingHouse = new PublishingHouseDto
                {
                    Id = publishingHouse.Id,
                    Name = publishingHouse.Name,
                    Country = publishingHouse.Country,
                    City = publishingHouse.City
                },
                Authors = await _context.BookAuthors
                    .Where(ba => ba.BookId == book.Id)
                    .Include(ba => ba.Author)
                    .Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}")
                    .ToListAsync(),
                Genres = await _context.BookGenres
                    .Where(bg => bg.BookId == book.Id)
                    .Include(bg => bg.Genre)
                    .Select(bg => bg.Genre.Name)
                    .ToListAsync()
            };

            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, response);
        }
    }
}
