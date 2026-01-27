using FluentAssertions;
using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.UnitTests.Common.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.UnitTests.Application.Features.Books.Commands
{
    public class DeleteBookCommandHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMessagePublisher> _messagePublisherMock;
        private readonly DeleteBookCommandHandler _handler;

        public DeleteBookCommandHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _messagePublisherMock = new Mock<IMessagePublisher>();

            _handler = new DeleteBookCommandHandler(
                _bookRepositoryMock.Object,
                _cacheServiceMock.Object,
                _messagePublisherMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithExistingBook_ShouldReturnTrue()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = BookFixtures.GetTestBook(id: bookId);

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var command = new DeleteBookCommand(bookId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WithNonExistingBook_ShouldReturnFalse()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LibraryManagement.Domain.Entities.Book?)null);

            var command = new DeleteBookCommand(bookId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_WithExistingBook_ShouldCallDeleteAsync()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = BookFixtures.GetTestBook(id: bookId);

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(
                r => r.DeleteAsync(bookId, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithNonExistingBook_ShouldNotCallDeleteAsync()
        {
            // Arrange
            var bookId = Guid.NewGuid();

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LibraryManagement.Domain.Entities.Book?)null);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _bookRepositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WithExistingBook_ShouldInvalidateCaches()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = BookFixtures.GetTestBook(id: bookId);

            _bookRepositoryMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cacheServiceMock.Verify(
                c => c.RemoveAsync($"books:{bookId}", It.IsAny<CancellationToken>()),
                Times.Once
            );
            _cacheServiceMock.Verify(
                c => c.RemoveAsync("books:all", It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}
