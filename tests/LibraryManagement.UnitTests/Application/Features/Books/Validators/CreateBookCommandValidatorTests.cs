using FluentAssertions;
using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LibraryManagement.UnitTests.Application.Features.Books.Validators
{
    public class CreateBookCommandValidatorTests
    {
        private readonly CreateBookCommandValidator _validator;

        public CreateBookCommandValidatorTests()
        {
            _validator = new CreateBookCommandValidator();
        }

        #region Valid Command Tests

        [Fact]
        public void Validate_WithValidCommand_ShouldHaveNoErrors()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Clean Architecture",
                Author: "Robert C. Martin",
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion

        #region Title Validation Tests

        [Fact]
        public void Validate_WithEmptyTitle_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "",
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_WithNullTitle_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: null!,
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_WithTitleExceeding200Characters_ShouldHaveError()
        {
            // Arrange
            var longTitle = new string('A', 201);
            var command = new CreateBookCommand(
                Title: longTitle,
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == "Title" &&
                e.ErrorMessage.Contains("200"));
        }

        #endregion

        #region Author Validation Tests

        [Fact]
        public void Validate_WithEmptyAuthor_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "",
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Author");
        }

        [Fact]
        public void Validate_WithAuthorExceeding100Characters_ShouldHaveError()
        {
            // Arrange
            var longAuthor = new string('A', 101);
            var command = new CreateBookCommand(
                Title: "Title",
                Author: longAuthor,
                ISBN: "9780134494166",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == "Author" &&
                e.ErrorMessage.Contains("100"));
        }

        #endregion

        #region ISBN Validation Tests

        [Fact]
        public void Validate_WithEmptyISBN_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: "",
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ISBN");
        }

        [Theory]
        [InlineData("9780134494166")]  // Valid 13-digit
        [InlineData("0134494164")]     // Valid 10-digit
        [InlineData("978-0-13-449416-6")] // Valid with dashes
        public void Validate_WithValidISBNFormats_ShouldHaveNoError(string isbn)
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: isbn,
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().NotContain(e =>
                e.PropertyName == "ISBN" &&
                e.ErrorMessage.Contains("valid format"));
        }

        [Theory]
        [InlineData("123")]           // Too short
        [InlineData("abcdefghij")]    // Letters
        [InlineData("12345678901234567890")] // Too long
        public void Validate_WithInvalidISBNFormats_ShouldHaveError(string isbn)
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: isbn,
                PublishedYear: 2017
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ISBN");
        }

        #endregion

        #region PublishedYear Validation Tests

        [Fact]
        public void Validate_WithYearBeforeBooks_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: 1400  // Before printing press
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PublishedYear");
        }

        [Fact]
        public void Validate_WithFutureYear_ShouldHaveError()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: DateTime.UtcNow.Year + 1
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PublishedYear");
        }

        [Theory]
        [InlineData(1450)]  // First printed books
        [InlineData(1900)]
        [InlineData(2020)]
        public void Validate_WithValidYears_ShouldHaveNoError(int year)
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "Title",
                Author: "Author",
                ISBN: "9780134494166",
                PublishedYear: year
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.Errors.Should().NotContain(e => e.PropertyName == "PublishedYear");
        }

        #endregion

        #region Multiple Errors Tests

        [Fact]
        public void Validate_WithMultipleInvalidFields_ShouldHaveMultipleErrors()
        {
            // Arrange
            var command = new CreateBookCommand(
                Title: "",
                Author: "",
                ISBN: "invalid",
                PublishedYear: 1200
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
        }

        #endregion
    }
}
