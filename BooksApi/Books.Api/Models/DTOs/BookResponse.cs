using System;
using System.Collections.Generic;

namespace Books.Api.Models.DTOs
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public PublishingHouseDto PublishingHouse { get; set; } = null!;
        public IEnumerable<string> Authors { get; set; } = new List<string>();
        public IEnumerable<string> Genres { get; set; } = new List<string>();
    }
}
