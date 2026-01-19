using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest.DAL.Repos
{
    public class BookRepository : IBookRepository
    {
        private readonly AppContext _context;

        public BookRepository(AppContext context)
        {
            _context = context;
        }
        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> BorrowBookAsync(int bookId, int userId, int daysToReturn = 14)
        {
            var book = await _context.Books.FindAsync(bookId);
            var user = await _context.Users.FindAsync(userId);

            if (book == null || user == null || book.BorrowedByUserId != null)
                return false;

            book.BorrowedByUserId = userId;
            book.BorrowedDate = DateTime.UtcNow;
            book.DueDate = DateTime.UtcNow.AddDays(daysToReturn);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await GetByIdAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<List<Book>> GetBusyBooksAsync()
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == null)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<List<Book>> GetBorrowedBooksAsync()
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId != null)
                .Include(b => b.BorrowedByUser)
                .OrderByDescending(b => b.BorrowedDate)
                .ToListAsync();
        }

        public async Task<List<Book>> GetByAuthorAsync(string author)
        {
            return await _context.Books
                .Where(b => b.Author != null && b.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<List<Book>> GetByGenreAsync(string genre)
        {
            return await _context.Books
                .Where(b => b.Genre.Contains(genre))
                .Include(b => b.BorrowedByUser)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<List<Book>> GetByYearAsync(int year)
        {
            return await _context.Books
                .Where(b => b.ReleaseYear == year)
                .ToListAsync();
        }

        public async Task<List<Book>> GetUserBorrowedBooksAsync(int userId)
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == userId)
                .OrderByDescending(b => b.BorrowedDate)
                .ToListAsync();
        }

        public async Task<bool> ReturnBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null || book.BorrowedByUserId == null)
                return false;

            book.BorrowedByUserId = null;
            book.BorrowedDate = null;
            book.DueDate = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(searchTerm) ||
                           b.Author.Contains(searchTerm) ||
                           b.Genre.Contains(searchTerm))
                .Include(b => b.BorrowedByUser)
                .ToListAsync();
        }

        public async Task<bool> UpdatePublicationYearAsync(int id, int newYear)
        {
            var book = await GetByIdAsync(id);
            if (book == null)
                return false;

            book.ReleaseYear = newYear;
            await _context.SaveChangesAsync();
            return true;
        }

        // Получать список книг определенного жанра и вышедших между определенными годами

        public async Task<List<Book>> GetBooksByGenreAndYearsAsync(string genre, int startYear, int endYear)
        {
            return await _context.Books
                .Where(b => b.Genre == genre &&
                           b.ReleaseYear >= startYear &&
                           b.ReleaseYear <= endYear)
                .Include(b => b.BorrowedByUser)
                .OrderBy(b => b.ReleaseYear)
                .ToListAsync();
        }

        // Получать количество книг определенного автора в библиотеке
        public async Task<int> GetBookCountByAuthorAsync(string author)
        {
            return await _context.Books
                .Where(b => b.Author == author)
                .CountAsync();
        }

        // Получать количество книг определенного жанра в библиотеке
        public async Task<int> GetBookCountByGenreAsync(string genre)
        {
            return await _context.Books
                .Where(b => b.Genre == genre)
                .CountAsync();
        }

        // Получать булевый флаг о том, есть ли книга определенного автора и с определенным названием в библиотеке

        public async Task<bool> BookExistsByAuthorAndTitleAsync(string author, string title)
        {
            return await _context.Books
                .AnyAsync(b => b.Author == author && b.Title == title);
        }

        // Получать булевый флаг о том, есть ли определенная книга на руках у пользователя
        public async Task<bool> IsBookBorrowedByUserAsync(int bookId, int userId)
        {
            return await _context.Books
                .AnyAsync(b => b.Id == bookId &&
                              b.BorrowedByUserId == userId);
        }

        // Получать количество книг на руках у пользователя
        public async Task<int> GetBorrowedBooksCountByUserAsync(int userId)
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == userId)
                .CountAsync();
        }

        // Получение последней вышедшей книги
        public async Task<Book?> GetLatestPublishedBookAsync()
        {
            return await _context.Books
                .Include(b => b.BorrowedByUser)
                .OrderByDescending(b => b.ReleaseYear)
                .ThenBy(b => b.Title)
                .FirstOrDefaultAsync();
        }

        // Получение списка всех книг, отсортированного в алфавитном порядке по названию
        public async Task<List<Book>> GetAllBooksSortedByTitleAsync()
        {
            return await _context.Books
                .Include(b => b.BorrowedByUser)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // Получение списка всех книг, отсортированного в порядке убывания года их выхода
        public async Task<List<Book>> GetAllBooksSortedByYearDescAsync()
        {
            return await _context.Books
                .Include(b => b.BorrowedByUser)
                .OrderByDescending(b => b.ReleaseYear)
                .ThenBy(b => b.Title)
                .ToListAsync();
        }
    }
}
