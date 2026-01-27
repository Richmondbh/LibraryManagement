using FluentAssertions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Domain.Entities;
using LibraryManagement.UnitTests.Common.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.UnitTests.Application.Features.Books.Commands
{
    public class CreateBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMessagePublisher> _messagePublisherMock;
        private readonly Mock<ITelemetryService> _telemetryServiceMock;
        private readonly CreateBookCommandHandler _handler;

        public CreateBookCommandHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _messagePublisherMock = new Mock<IMessagePublisher>();
            _telemetryServiceMock = new Mock<ITelemetryService>(); 

            _handler = new CreateBookCommandHandler(
                _bookRepositoryMock.Object,
                _cacheServiceMock.Object,
                _messagePublisherMock.Object,
                 _telemetryServiceMock.Object

            );
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnBookId()
        {
            // Arrange
            var command = BookFixtures.GetValidCreateCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCallRepositoryAddAsync()
        {
            // Arrange
            var command = BookFixtures.GetValidCreateCommand();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldInvalidateCache()
        {
            // Arrange
            var command = BookFixtures.GetValidCreateCommand();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cacheServiceMock.Verify(
                c => c.RemoveAsync("books:all", It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldPublishEvent()
        {
            // Arrange
            var command = BookFixtures.GetValidCreateCommand();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _messagePublisherMock.Verify(
                m => m.PublishAsync(
                    It.IsAny<object>(),
                    "book-created",
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateBookWithCorrectProperties()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Test Title",
                Author: "Test Author",
                ISBN: "1234567890",
                PublishedYear: 2023
            );

            Book? savedBook = null;
            _bookRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .Callback<Book, CancellationToken>((book, _) => savedBook = book);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            savedBook.Should().NotBeNull();
            savedBook!.Title.Should().Be(command.Title);
            savedBook.Author.Should().Be(command.Author);
            savedBook.ISBN.Should().Be(command.ISBN);
            savedBook.PublishedYear.Should().Be(command.PublishedYear);
        }
    }
}
