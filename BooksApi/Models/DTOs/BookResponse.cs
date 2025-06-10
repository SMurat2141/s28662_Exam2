using System;
using System.Collections.Generic;

namespace BooksApi.Models.DTOs
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public PublishingHouseDto PublishingHouse { get; set; } = null!;
        public List<string> Authors { get; set; } = new();
        public List<string> Genres { get; set; } = new();
    }
}
