using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.UploadBookCover
{
    public class UploadBookCoverCommandHandler : IRequestHandler<UploadBookCoverCommand, string?>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ICacheService _cacheService;

        public UploadBookCoverCommandHandler(
            IBookRepository bookRepository,
            IBlobStorageService blobStorageService,
            ICacheService cacheService)
        {
            _bookRepository = bookRepository;
            _blobStorageService = blobStorageService;
            _cacheService = cacheService;
        }

        public async Task<string?> Handle(UploadBookCoverCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);

            if (book is null)
                return null;

            // Generate unique filename
            var fileExtension = Path.GetExtension(request.FileName);
            var blobFileName = $"{request.BookId}{fileExtension}";

            // Upload to blob storage
            var imageUrl = await _blobStorageService.UploadAsync(
                request.FileStream,
                blobFileName,
                request.ContentType,
                cancellationToken);

            // Update book with cover URL
            book.SetCoverImage(imageUrl);
            await _bookRepository.UpdateAsync(book, cancellationToken);

            // Invalidate cache
            await _cacheService.RemoveAsync($"books:{request.BookId}", cancellationToken);
            await _cacheService.RemoveAsync("books:all", cancellationToken);

            return imageUrl;
        }
    }
}
