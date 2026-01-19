using EFTest.DAL.Repos;
using Microsoft.EntityFrameworkCore;

namespace EFTest
{
    class Programm
    {
        static async Task Main(string[] args)
        {
            // Создаем базу данных
            using var context = new AppContext();
            await context.Database.EnsureCreatedAsync();

            // Создаем репозитории
            IUserRepository userRepository = new UserRepository(context);
            IBookRepository bookRepository = new BookRepository(context);

            Console.WriteLine("=== ДЕМОНСТРАЦИЯ НОВЫХ МЕТОДОВ РЕПОЗИТОРИЯ ===\n");

            // Создание тестовых данных
            await CreateTestData(userRepository, bookRepository);

            try
            {
                // 1. Получать список книг определенного жанра и вышедших между определенными годами
                Console.WriteLine("1. КНИГИ ЖАНРА 'Роман' ВЫШЕДШИЕ МЕЖДУ 1800 и 1900 ГОДАМИ:");
                var romanBooks1800_1900 = await bookRepository.GetBooksByGenreAndYearsAsync("Роман", 1800, 1900);
                foreach (var book in romanBooks1800_1900)
                {
                    Console.WriteLine($"   - {book.Title} ({book.ReleaseYear})");
                }
                Console.WriteLine($"   Всего найдено: {romanBooks1800_1900.Count}\n");

                // 2. Получать количество книг определенного автора в библиотеке
                Console.WriteLine("2. КОЛИЧЕСТВО КНИГ АВТОРА 'Лев Толстой':");
                var tolstoyCount = await bookRepository.GetBookCountByAuthorAsync("Лев Толстой");
                Console.WriteLine($"   У Толстого книг: {tolstoyCount}\n");

                // 3. Получать количество книг определенного жанра в библиотеке
                Console.WriteLine("3. КОЛИЧЕСТВО КНИГ ЖАНРА 'Фэнтези':");
                var fantasyCount = await bookRepository.GetBookCountByGenreAsync("Фэнтези");
                Console.WriteLine($"   Фэнтези книг: {fantasyCount}\n");

                // 4. Проверка наличия книги определенного автора и названия
                Console.WriteLine("4. ПРОВЕРКА НАЛИЧИЯ КНИГИ:");
                var bookExists1 = await bookRepository.BookExistsByAuthorAndTitleAsync("Лев Толстой", "Война и мир");
                var bookExists2 = await bookRepository.BookExistsByAuthorAndTitleAsync("Лев Толстой", "Несуществующая книга");
                Console.WriteLine($"   'Война и мир' Толстого есть: {bookExists1}");
                Console.WriteLine($"   'Несуществующая книга' Толстого есть: {bookExists2}\n");

                // 5. Проверка, есть ли книга на руках у пользователя
                Console.WriteLine("5. ПРОВЕРКА КНИГИ У ПОЛЬЗОВАТЕЛЯ:");
                var isBookBorrowed = await bookRepository.IsBookBorrowedByUserAsync(1, 1); // Книга 1 у пользователя 1
                Console.WriteLine($"   Книга ID=1 у пользователя ID=1: {isBookBorrowed}\n");

                // 6. Количество книг на руках у пользователя
                Console.WriteLine("6. КОЛИЧЕСТВО КНИГ У ПОЛЬЗОВАТЕЛЯ:");
                var borrowedCount1 = await bookRepository.GetBorrowedBooksCountByUserAsync(1);
                var borrowedCount2 = await userRepository.GetBorrowedBooksCountAsync(1); // Альтернативный метод
                Console.WriteLine($"   У пользователя ID=1 книг (метод BookRepository): {borrowedCount1}");
                Console.WriteLine($"   У пользователя ID=1 книг (метод UserRepository): {borrowedCount2}\n");

                // 7. Последняя вышедшая книга
                Console.WriteLine("7. ПОСЛЕДНЯЯ ВЫШЕДШАЯ КНИГА:");
                var latestBook = await bookRepository.GetLatestPublishedBookAsync();
                if (latestBook != null)
                {
                    Console.WriteLine($"   Последняя книга: {latestBook.Title} ({latestBook.ReleaseYear})");
                }
                Console.WriteLine();

                // 8. Список всех книг по алфавиту
                Console.WriteLine("8. ВСЕ КНИГИ ПО АЛФАВИТУ:");
                var booksByTitle = await bookRepository.GetAllBooksSortedByTitleAsync();
                foreach (var book in booksByTitle.Take(5)) // Покажем первые 5
                {
                    Console.WriteLine($"   - {book.Title}");
                }
                Console.WriteLine($"   ... и еще {booksByTitle.Count - 5} книг\n");

                // 9. Список всех книг по убыванию года
                Console.WriteLine("9. КНИГИ ПО УБЫВАНИЮ ГОДА ВЫПУСКА:");
                var booksByYearDesc = await bookRepository.GetAllBooksSortedByYearDescAsync();
                foreach (var book in booksByYearDesc)
                {
                    Console.WriteLine($"   - {book.ReleaseYear}: {book.Title}");
                }
                Console.WriteLine();

                // 10. Дополнительные демонстрации
                Console.WriteLine("10. ДОПОЛНИТЕЛЬНАЯ СТАТИСТИКА:");

                // Книги жанра "Детектив" между 1900 и 2000
                var detectiveBooks = await bookRepository.GetBooksByGenreAndYearsAsync("Детектив", 1900, 2000);
                Console.WriteLine($"   Детективов 1900-2000: {detectiveBooks.Count}");

                // Количество всех книг Достоевского
                var dostoevskyCount = await bookRepository.GetBookCountByAuthorAsync("Федор Достоевский");
                Console.WriteLine($"   Книг Достоевского: {dostoevskyCount}");

                // Проверка всех пользователей
                var users = await userRepository.GetAllAsync();
                foreach (var user in users)
                {
                    var userBookCount = await bookRepository.GetBorrowedBooksCountByUserAsync(user.Id);
                    Console.WriteLine($"   {user.Name}: {userBookCount} книг на руках");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\n=== ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА ===");
            Console.ReadKey();
        }

        static async Task CreateTestData(IUserRepository userRepo, IBookRepository bookRepo)
        {
            Console.WriteLine("Создание тестовых данных...\n");

            // Пользователи
            var user1 = await userRepo.AddAsync(new User { Name = "Иван Иванов", Email = "ivan@mail.com" });
            var user2 = await userRepo.AddAsync(new User { Name = "Мария Петрова", Email = "maria@mail.com" });

            // Книги
            var books = new List<Book>
        {
            new Book { Title = "Война и мир", ReleaseYear = 1869, Author = "Лев Толстой", Genre = "Роман" },
            new Book { Title = "Анна Каренина", ReleaseYear = 1877, Author = "Лев Толстой", Genre = "Роман" },
            new Book { Title = "Преступление и наказание", ReleaseYear = 1866, Author = "Федор Достоевский", Genre = "Роман" },
            new Book { Title = "Идиот", ReleaseYear = 1869, Author = "Федор Достоевский", Genre = "Роман" },
            new Book { Title = "1984", ReleaseYear = 1949, Author = "Джордж Оруэлл", Genre = "Антиутопия" },
            new Book { Title = "Скотный двор", ReleaseYear = 1945, Author = "Джордж Оруэлл", Genre = "Сатира" },
            new Book { Title = "Гарри Поттер и философский камень", ReleaseYear = 1997, Author = "Джоан Роулинг", Genre = "Фэнтези" },
            new Book { Title = "Гарри Поттер и Тайная комната", ReleaseYear = 1998, Author = "Джоан Роулинг", Genre = "Фэнтези" },
            new Book { Title = "Убийство в Восточном экспрессе", ReleaseYear = 1934, Author = "Агата Кристи", Genre = "Детектив" },
            new Book { Title = "Десять негритят", ReleaseYear = 1939, Author = "Агата Кристи", Genre = "Детектив" },
            new Book { Title = "Мастер и Маргарита", ReleaseYear = 1967, Author = "Михаил Булгаков", Genre = "Роман" }
        };

            foreach (var book in books)
            {
                await bookRepo.AddAsync(book);
            }

            // Выдаем некоторые книги
            await bookRepo.BorrowBookAsync(1, user1.Id);  // Война и мир → Иван
            await bookRepo.BorrowBookAsync(7, user1.Id);  // Гарри Поттер 1 → Иван
            await bookRepo.BorrowBookAsync(9, user2.Id);  // Убийство в Восточном экспрессе → Мария
        }
    }
}