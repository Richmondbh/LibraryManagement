using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler: IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    public DeleteBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);

        if (book is null)
            return false;

        await _bookRepository.DeleteAsync(request.Id, cancellationToken);

        // Invalidate caches
        await _cacheService.RemoveAsync($"books:{request.Id}", cancellationToken);
        await _cacheService.RemoveAsync("books:all", cancellationToken);

        return true;
    }
}
