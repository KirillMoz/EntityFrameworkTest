using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
    }
}
