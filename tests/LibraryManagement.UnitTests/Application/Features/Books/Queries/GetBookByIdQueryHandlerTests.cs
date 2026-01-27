using FluentAssertions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Features.Books.Queries.GetBookById;
using LibraryManagement.UnitTests.Common.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.UnitTests.Application.Features.Books.Queries
{
    public class GetBookByIdQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly GetBookByIdQueryHandler _handler;

        public GetBookByIdQueryHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new GetBookByIdQueryHandler(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingBook_ShouldReturnBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = BookFixtures.GetTestBook(id: bookId, title: "Test Book");

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var query = new GetBookByIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Title.Should().Be("Test Book");
        }

        [Fact]
        public async Task Handle_WithNonExistingBook_ShouldReturnNull()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LibraryManagement.Domain.Entities.Book?)null);

            var query = new GetBookByIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = BookFixtures.GetTestBook(
                id: bookId,
                title: "Clean Code",
                author: "Robert Martin",
                isbn: "9780132350884",
                publishedYear: 2008
            );
            book.SetCoverImage("https://example.com/cover.jpg");

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var query = new GetBookByIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(bookId);
            result.Title.Should().Be("Clean Code");
            result.Author.Should().Be("Robert Martin");
            result.ISBN.Should().Be("9780132350884");
            result.PublishedYear.Should().Be(2008);
            result.CoverImageUrl.Should().Be("https://example.com/cover.jpg");
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryWithCorrectId()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LibraryManagement.Domain.Entities.Book?)null);

            var query = new GetBookByIdQuery(bookId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(
                r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
