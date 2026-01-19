using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest
{
    public class AppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        public AppContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-KJKS2CJ\SQLEXPRESS;Database=TestEF;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация для User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Конфигурация для Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Genre).IsRequired().HasMaxLength(50);
                entity.Property(b => b.ReleaseYear)
                    .IsRequired()
                    .HasAnnotation("Range", new[] { 1000, 2100 });

                // Связь один-ко-многим: один пользователь может брать много книг
                entity.HasOne(b => b.BorrowedByUser)
                      .WithMany(u => u.BorrowedBooks)
                      .HasForeignKey(b => b.BorrowedByUserId)
                      .OnDelete(DeleteBehavior.SetNull); // При удалении пользователя книги освобождаются
            });
        }
    }
}
