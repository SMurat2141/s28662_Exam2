using System.Collections.Generic;

namespace BooksApi.Models.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
