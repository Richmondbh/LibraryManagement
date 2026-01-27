using FluentAssertions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.UnitTests.Common.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.UnitTests.Application.Features.Books.Queries
{
    public class GetAllBooksQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly GetAllBooksQueryHandler _handler;

        public GetAllBooksQueryHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new GetAllBooksQueryHandler(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenBooksExist_ShouldReturnAllBooks()
        {
            // Arrange
            var books = BookFixtures.GetTestBooks(3);
            _bookRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(books);

            var query = new GetAllBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_WhenNoBooksExist_ShouldReturnEmptyList()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<LibraryManagement.Domain.Entities.Book>());

            var query = new GetAllBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldMapBookPropertiesCorrectly()
        {
            // Arrange
            var book = BookFixtures.GetTestBook(
                title: "Test Title",
                author: "Test Author",
                isbn: "1234567890",
                publishedYear: 2023
            );

            _bookRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<LibraryManagement.Domain.Entities.Book> { book });

            var query = new GetAllBooksQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var bookResponse = result.First();
            bookResponse.Title.Should().Be("Test Title");
            bookResponse.Author.Should().Be("Test Author");
            bookResponse.ISBN.Should().Be("1234567890");
            bookResponse.PublishedYear.Should().Be(2023);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnce()
        {
            // Arrange
            _bookRepositoryMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<LibraryManagement.Domain.Entities.Book>());

            var query = new GetAllBooksQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(
                r => r.GetAllAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
