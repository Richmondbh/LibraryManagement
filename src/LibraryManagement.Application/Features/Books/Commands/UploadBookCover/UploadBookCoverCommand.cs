using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.UploadBookCover;

public record UploadBookCoverCommand(
    Guid BookId,
    Stream FileStream,
    string FileName,
    string ContentType
) : IRequest<string?>;

