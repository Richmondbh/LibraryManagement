using FluentAssertions;
using LibraryManagement.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static LibraryManagement.IntegrationTests.Common.TestDtos;

namespace LibraryManagement.IntegrationTests.Controllers
{
    public class BooksControllerTests : BaseIntegrationTest
    {
        private const string BaseUrl = "/api/books";

        public BooksControllerTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region Helper Method

        private async Task<Guid> CreateTestBookAsync(string title = "Test Book", string author = "Test Author")
        {
            var request = new CreateBookRequest(title, author, "9780134494166", 2020);
            var response = await Client.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Guid>();
        }

        #endregion

        #region GET /api/books Tests

        [Fact]
        public async Task GetAll_WhenNoBooksExist_ShouldReturnOk()
        {
            // Act
            var response = await Client.GetAsync(BaseUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_WhenBooksExist_ShouldReturnAllBooks()
        {
            // Arrange
            await CreateTestBookAsync("Book 1", "Author 1");
            await CreateTestBookAsync("Book 2", "Author 2");

            // Act
            var response = await Client.GetAsync(BaseUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
            books.Should().NotBeNull();
            books!.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        #endregion

        #region GET /api/books/{id} Tests

        [Fact]
        public async Task GetById_WithExistingBook_ShouldReturnBook()
        {
            // Arrange
            var bookId = await CreateTestBookAsync("Test Book For GetById", "Test Author");

            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{bookId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var book = await response.Content.ReadFromJsonAsync<BookResponse>();
            book.Should().NotBeNull();
            book!.Id.Should().Be(bookId);
            book.Title.Should().Be("Test Book For GetById");
        }

        [Fact]
        public async Task GetById_WithNonExistingBook_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await Client.GetAsync($"{BaseUrl}/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region POST /api/books Tests

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateBookRequest(
                "Clean Architecture",
                "Robert C. Martin",
                "9780134494166",
                2017
            );

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var bookId = await response.Content.ReadFromJsonAsync<Guid>();
            bookId.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Create_WithValidData_ShouldPersistBook()
        {
            // Arrange
            var request = new CreateBookRequest(
                "Domain-Driven Design",
                "Eric Evans",
                "9780321125217",
                2003
            );

            // Act
            var createResponse = await Client.PostAsJsonAsync(BaseUrl, request);
            var bookId = await createResponse.Content.ReadFromJsonAsync<Guid>();

            // Assert
            var getResponse = await Client.GetAsync($"{BaseUrl}/{bookId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var book = await getResponse.Content.ReadFromJsonAsync<BookResponse>();
            book.Should().NotBeNull();
            book!.Title.Should().Be("Domain-Driven Design");
        }

        [Fact]
        public async Task Create_WithEmptyTitle_ShouldRejectRequest()
        {
            // Arrange
            var request = new CreateBookRequest("", "Author", "9780134494166", 2020);

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert - Request should fail (validation rejects it)
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Create_WithEmptyAuthor_ShouldRejectRequest()
        {
            // Arrange
            var request = new CreateBookRequest("Title", "", "9780134494166", 2020);

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert - Request should fail (validation rejects it)
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Create_WithInvalidYear_ShouldRejectRequest()
        {
            // Arrange
            var request = new CreateBookRequest("Title", "Author", "9780134494166", 1200);

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, request);

            // Assert - Request should fail (validation rejects it)
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        #endregion

        #region PUT /api/books/{id} Tests

        [Fact]
        public async Task Update_WithValidData_ShouldReturnNoContent()
        {
            // Arrange
            var bookId = await CreateTestBookAsync("Original Title", "Original Author");

            var updateRequest = new UpdateBookRequest(
                bookId,
                "Updated Title",
                "Updated Author",
                "0987654321",
                2023
            );

            // Act
            var response = await Client.PutAsJsonAsync($"{BaseUrl}/{bookId}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Update_WithValidData_ShouldPersistChanges()
        {
            // Arrange
            var bookId = await CreateTestBookAsync("Original Title", "Original Author");

            var updateRequest = new UpdateBookRequest(
                bookId,
                "Updated Title",
                "Updated Author",
                "0987654321",
                2023
            );

            // Act
            await Client.PutAsJsonAsync($"{BaseUrl}/{bookId}", updateRequest);

            // Assert
            var getResponse = await Client.GetAsync($"{BaseUrl}/{bookId}");
            var book = await getResponse.Content.ReadFromJsonAsync<BookResponse>();

            book.Should().NotBeNull();
            book!.Title.Should().Be("Updated Title");
            book.Author.Should().Be("Updated Author");
        }

        [Fact]
        public async Task Update_WithNonExistingBook_ShouldReturnNotFoundOrBadRequest()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var updateRequest = new UpdateBookRequest(
                nonExistingId,
                "Title",
                "Author",
                "1234567890",
                2020
            );

            // Act
            var response = await Client.PutAsJsonAsync($"{BaseUrl}/{nonExistingId}", updateRequest);

            // Assert - Could be NotFound or BadRequest depending on validation order
            response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
        }

        #endregion

        #region DELETE /api/books/{id} Tests

        [Fact]
        public async Task Delete_WithExistingBook_ShouldReturnNoContent()
        {
            // Arrange
            var bookId = await CreateTestBookAsync("Book to Delete", "Author");

            // Act
            var response = await Client.DeleteAsync($"{BaseUrl}/{bookId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_WithExistingBook_ShouldRemoveBook()
        {
            // Arrange
            var bookId = await CreateTestBookAsync("Book to Delete", "Author");

            // Act
            await Client.DeleteAsync($"{BaseUrl}/{bookId}");

            // Assert
            var getResponse = await Client.GetAsync($"{BaseUrl}/{bookId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WithNonExistingBook_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await Client.DeleteAsync($"{BaseUrl}/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion
    }
}