using System;
using System.Collections.Generic;

namespace Books.Api.Models.DTOs
{
    public class BookCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public PublishingHouseDto PublishingHouse { get; set; } = null!;
        public List<int> AuthorIds { get; set; } = new();
        public List<int> GenreIds { get; set; } = new();
    }
}
