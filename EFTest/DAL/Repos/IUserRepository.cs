using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest.DAL.Repos
{
    public interface IUserRepository: IBaseRepository<User>
    {
        Task<bool> UpdateNameAsync(int id, string newName);
        Task<User?> GetByEmailAsync(string email);
        Task<List<Book>> GetBorrowedBooksAsync(int userId);
        Task<bool> HasOverdueBooksAsync(int userId);

        // Новый метод подсчета книг у пользователя
        Task<int> GetBorrowedBooksCountAsync(int userId);
    }
}
