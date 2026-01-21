using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Data.Repositories
{
    public class PostGresBookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;
        public PostGresBookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Books
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        {
            await _context.Books.AddAsync(book, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var book = await GetByIdAsync(id, cancellationToken);
            if (book is not null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
    
}
