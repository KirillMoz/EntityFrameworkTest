using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest.DAL.Repos
{
    public interface IBookRepository: IBaseRepository<Book>
    {
        Task<bool> UpdatePublicationYearAsync(int id, int newYear);
        Task<List<Book>> GetByYearAsync(int year);
        Task<List<Book>> GetByAuthorAsync(string author);
        Task<List<Book>> GetByGenreAsync(string genre);
        Task<List<Book>> GetBusyBooksAsync();
        Task<List<Book>> GetBorrowedBooksAsync();
        Task<bool> BorrowBookAsync(int bookId, int userId, int daysToReturn = 14);
        Task<bool> ReturnBookAsync(int bookId);
        Task<List<Book>> GetUserBorrowedBooksAsync(int userId);
        Task<List<Book>> SearchBooksAsync(string searchTerm);

        Task<List<Book>> GetBooksByGenreAndYearsAsync(string genre, int startYear, int endYear);
        Task<int> GetBookCountByAuthorAsync(string author);
        Task<int> GetBookCountByGenreAsync(string genre);
        Task<bool> BookExistsByAuthorAndTitleAsync(string author, string title);
        Task<bool> IsBookBorrowedByUserAsync(int bookId, int userId);
        Task<int> GetBorrowedBooksCountByUserAsync(int userId);
        Task<Book?> GetLatestPublishedBookAsync();
        Task<List<Book>> GetAllBooksSortedByTitleAsync();
        Task<List<Book>> GetAllBooksSortedByYearDescAsync();
    }
}
