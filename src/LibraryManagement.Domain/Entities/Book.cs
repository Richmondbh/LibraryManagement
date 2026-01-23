using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Author { get; private set; } = string.Empty;
        public string ISBN { get; private set; } = string.Empty;
        public int PublishedYear { get; private set; }
        public string? CoverImageUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Book() { } 

        public static Book Create(string title, string author, string isbn, int publishedYear)
        {
            return new Book
            {
                Id = Guid.NewGuid(),
                Title = title,
                Author = author,
                ISBN = isbn,
                PublishedYear = publishedYear,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(string title, string author, string isbn, int publishedYear)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            PublishedYear = publishedYear;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetCoverImage(string coverImageUrl)
        {
            CoverImageUrl = coverImageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
