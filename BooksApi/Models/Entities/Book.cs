using System;
using System.Collections.Generic;

namespace BooksApi.Models.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }

        public int PublishingHouseId { get; set; }
        public PublishingHouse PublishingHouse { get; set; } = null!;

        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}
