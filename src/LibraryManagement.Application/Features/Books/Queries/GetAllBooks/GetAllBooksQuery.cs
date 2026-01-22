using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks;

public record GetAllBooksQuery : IRequest<IEnumerable<BookResponse>>, ICacheableQuery 
{

    public string CacheKey => "books:all";
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);

}
