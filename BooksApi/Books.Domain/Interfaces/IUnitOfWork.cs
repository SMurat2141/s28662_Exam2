using System.Threading.Tasks;
using Books.Domain.Entities;

namespace Books.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Book> Books { get; }
        IRepository<Author> Authors { get; }
        IRepository<Genre> Genres { get; }
        IRepository<PublishingHouse> PublishingHouses { get; }
        Task<int> SaveChangesAsync();
    }
}
