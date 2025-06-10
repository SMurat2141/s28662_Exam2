using System;
using System.Collections.Generic;

namespace Books.Api.Models.DTOs
{
    public class BookUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int PublishingHouseId { get; set; }
        public List<int> AuthorIds { get; set; } = new();
        public List<int> GenreIds { get; set; } = new();
    }
}
