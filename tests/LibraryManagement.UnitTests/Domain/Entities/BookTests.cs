using FluentAssertions;
using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibraryManagement.UnitTests.Domain.Entities
{
    public class BookTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidData_ReturnsBook()
        {
            // Arrange
            var title = "Clean Architecture";
            var author = "Robert C. Martin";
            var isbn = "978-0134494166";
            var publishedYear = 2017;

            // Act
            var book = Book.Create(title, author, isbn, publishedYear);

            // Assert
            book.Should().NotBeNull();
            book.Title.Should().Be(title);
            book.Author.Should().Be(author);
            book.ISBN.Should().Be(isbn);
            book.PublishedYear.Should().Be(publishedYear);
        }

        [Fact]
        public void Create_ShouldGenerateNewId()
        {
            // Arrange & Act
            var book = Book.Create("Title", "Author", "1234567890", 2020);

            // Assert
            book.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Create_ShouldSetCreatedAt()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var book = Book.Create("Title", "Author", "1234567890", 2020);

            // Assert
            book.CreatedAt.Should().BeOnOrAfter(beforeCreation);
            book.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Create_ShouldHaveNullUpdatedAt()
        {
            // Arrange & Act
            var book = Book.Create("Title", "Author", "1234567890", 2020);

            // Assert
            book.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Create_ShouldHaveNullCoverImageUrl()
        {
            // Arrange & Act
            var book = Book.Create("Title", "Author", "1234567890", 2020);

            // Assert
            book.CoverImageUrl.Should().BeNull();
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithValidData_ShouldUpdateProperties()
        {
            // Arrange
            var book = Book.Create("Old Title", "Old Author", "1111111111", 2000);
            var newTitle = "New Title";
            var newAuthor = "New Author";
            var newIsbn = "2222222222";
            var newYear = 2023;

            // Act
            book.Update(newTitle, newAuthor, newIsbn, newYear);

            // Assert
            book.Title.Should().Be(newTitle);
            book.Author.Should().Be(newAuthor);
            book.ISBN.Should().Be(newIsbn);
            book.PublishedYear.Should().Be(newYear);
        }

        [Fact]
        public void Update_ShouldSetUpdatedAt()
        {
            // Arrange
            var book = Book.Create("Title", "Author", "1234567890", 2020);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            book.Update("New Title", "New Author", "0987654321", 2023);

            // Assert
            book.UpdatedAt.Should().NotBeNull();
            book.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        }

        [Fact]
        public void Update_ShouldNotChangeId()
        {
            // Arrange
            var book = Book.Create("Title", "Author", "1234567890", 2020);
            var originalId = book.Id;

            // Act
            book.Update("New Title", "New Author", "0987654321", 2023);

            // Assert
            book.Id.Should().Be(originalId);
        }

        [Fact]
        public void Update_ShouldNotChangeCreatedAt()
        {
            // Arrange
            var book = Book.Create("Title", "Author", "1234567890", 2020);
            var originalCreatedAt = book.CreatedAt;

            // Act
            book.Update("New Title", "New Author", "0987654321", 2023);

            // Assert
            book.CreatedAt.Should().Be(originalCreatedAt);
        }

        #endregion

        #region SetCoverImage Tests

        [Fact]
        public void SetCoverImage_WithValidUrl_ShouldSetCoverImageUrl()
        {
            // Arrange
            var book = Book.Create("Title", "Author", "1234567890", 2020);
            var coverUrl = "https://storage.blob.core.windows.net/covers/book.jpg";

            // Act
            book.SetCoverImage(coverUrl);

            // Assert
            book.CoverImageUrl.Should().Be(coverUrl);
        }

        [Fact]
        public void SetCoverImage_ShouldSetUpdatedAt()
        {
            // Arrange
            var book = Book.Create("Title", "Author", "1234567890", 2020);
            var beforeUpdate = DateTime.UtcNow;

            // Act
            book.SetCoverImage("https://example.com/cover.jpg");

            // Assert
            book.UpdatedAt.Should().NotBeNull();
            book.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        }

        #endregion
    }
}
