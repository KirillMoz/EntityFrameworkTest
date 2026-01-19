using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }

        public int GenreId { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        // Внешний ключ для связи с пользователем
        public int? BorrowedByUserId { get; set; }

        // Навигационное свойство для пользователя
        public User? BorrowedByUser { get; set; }

        // Дата, когда книга была взята
        public DateTime? BorrowedDate { get; set; }

        // Дата, когда книга должна быть возвращена
        public DateTime? DueDate { get; set; }

        public bool IsBusy => BorrowedByUserId == null;
    }
}
