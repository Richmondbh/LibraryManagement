using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.UnitTests.Common.Fixtures
{
    public class BookFixtures
    {
        public static Book GetTestBook(
        Guid? id = null,
        string title = "Test Book",
        string author = "Test Author",
        string isbn = "9780134494166",
        int publishedYear = 2020)
        {
            var book = Book.Create(title, author, isbn, publishedYear);

            // Using  reflection to set Id if provided (for testing specific IDs)
            if (id.HasValue)
            {
                var idProperty = typeof(Book).GetProperty("Id");
                idProperty?.SetValue(book, id.Value);
            }

            return book;
        }

        public static CreateBookCommand GetValidCreateCommand(
            string title = "Good Man",
            string author = "Richmond",
            string isbn = "978013449499",
            int publishedYear = 2017)
        {
            return new CreateBookCommand(title, author, isbn, publishedYear);
        }

        public static UpdateBookCommand GetValidUpdateCommand(
            Guid id,
            string title = "Updated Title",
            string author = "Updated Author",
            string isbn = "9780134494166",
            int publishedYear = 2023)
        {
            return new UpdateBookCommand(id, title, author, isbn, publishedYear);
        }

        public static List<Book> GetTestBooks(int count = 3)
        {
            var books = new List<Book>();
            for (int i = 1; i <= count; i++)
            {
                books.Add(Book.Create(
                    $"Book {i}",
                    $"Author {i}",
                    $"978013449416{i}",
                    2020 + i
                ));
            }
            return books;
        }
    }
}
