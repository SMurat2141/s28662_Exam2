using System.Threading.Tasks;
using Books.Domain.Entities;
using Books.Domain.Interfaces;
using Books.Infrastructure.Data;

namespace Books.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BooksDbContext _context;

        public UnitOfWork(BooksDbContext context)
        {
            _context = context;
            Books = new Repository<Book>(_context);
            Authors = new Repository<Author>(_context);
            Genres = new Repository<Genre>(_context);
            PublishingHouses = new Repository<PublishingHouse>(_context);
        }

        public IRepository<Book> Books { get; }
        public IRepository<Author> Authors { get; }
        public IRepository<Genre> Genres { get; }
        public IRepository<PublishingHouse> PublishingHouses { get; }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
