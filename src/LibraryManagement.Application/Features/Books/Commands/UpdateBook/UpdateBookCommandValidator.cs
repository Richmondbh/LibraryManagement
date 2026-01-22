using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandValidator :  AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Book ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required")
                .MaximumLength(100).WithMessage("Author must not exceed 100 characters");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .MaximumLength(20).WithMessage("ISBN must not exceed 20 characters")
                .Matches(@"^(?:\d{10}|\d{13}|[\d-]{13,17})$")
                    .WithMessage("ISBN must be a valid format (10 or 13 digits)");

            RuleFor(x => x.PublishedYear)
                .InclusiveBetween(1450, DateTime.UtcNow.Year)
                    .WithMessage($"Published year must be between 1450 and {DateTime.UtcNow.Year}");
        }
    }
}
