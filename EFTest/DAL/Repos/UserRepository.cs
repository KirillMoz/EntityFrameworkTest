using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest.DAL.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly AppContext _context;
        public UserRepository(AppContext context) 
        {
            _context = context;
        }

        public async Task<User> AddAsync(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<Book>> GetBorrowedBooksAsync(int userId)
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == userId)
                .OrderByDescending(b => b.BorrowedDate)
                .ToListAsync();
        }
        // получать количество книг на руках у пользователя
        public async Task<int> GetBorrowedBooksCountAsync(int userId)
        {
            return await _context.Books
                .Where(b => b.BorrowedByUserId == userId)
                .CountAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> HasOverdueBooksAsync(int userId)
        {
            return await _context.Books
                .AnyAsync(b => b.BorrowedByUserId == userId &&
                              b.DueDate.HasValue &&
                              b.DueDate.Value < DateTime.UtcNow);
        }

        public async Task<bool> UpdateNameAsync(int id, string newName)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                return false;

            user.Name = newName;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
