using LibroConsoleAPI.Business.Contracts;
using LibroConsoleAPI.Data.Models;
using LibroConsoleAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LibroConsoleAPI.IntegrationTests
{
    public class IntegrationTests : IClassFixture<BookManagerFixture>
    {
        private readonly BookManager _bookManager;
        private readonly TestLibroDbContext _dbContext;

        public IntegrationTests(BookManagerFixture fixture)
        {
            _bookManager = fixture.BookManager;
            _dbContext = fixture.DbContext;
        }

        [Fact]
        public async Task AddBookAsync_ShouldAddBook()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            // Act
            await _bookManager.AddAsync(newBook);

            // Assert
            var bookInDbUsingBookmanager = _bookManager.GetSpecificAsync(newBook.ISBN);
            var bookInDb = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN);
            Assert.NotNull(bookInDb);
            Assert.Equal("Test Book", bookInDb.Title);
            Assert.Equal("John Doe", bookInDb.Author);
        }

        [Fact]
        public async Task AddBookAsync_WHenPassInvalidTitle_ShouldThrowException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = new string('A', 500),
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => _bookManager.AddAsync(newBook));

            // Assert
            var bookInDb = await _dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == newBook.ISBN);
            Assert.Equal("Book is invalid.", exception.Result.Message);
            
        }

        [Fact]
        public async Task AddBookAsync_WHenPassInvalidISBN_ShouldThrowException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Some book title",
                Author = "John Doe",
                ISBN = "bashbcd",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };

            // Act
            var exception = Assert.ThrowsAsync<ValidationException>(() => _bookManager.AddAsync(newBook));

            // Assert
            var bookInDb = await _dbContext.Books.FirstOrDefaultAsync();
            Assert.Equal("Book is invalid.", exception.Result.Message);

        }

        [Fact]
        public async Task DeleteBookAsync_WithValidISBN_ShouldRemoveBookFromDb()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Some book title",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            await _bookManager.AddAsync(newBook);

            // Act
            await _bookManager.DeleteAsync(newBook.ISBN);
            
            // Assert
            var bookInDb = await _dbContext.Books.FirstOrDefaultAsync();
            Assert.Null(bookInDb);
        }
        [Fact]
        public async Task DeleteBookAsync_TryToDeleteWithNullOrWhiteSpaceISBN_ShouldThrowException()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public async Task GetAllAsync_WhenBooksExist_ShouldReturnAllBooks()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);

            // Act
            var result = _bookManager.GetAllAsync();

            // Assert
            Assert.Equal(10, result.Result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoBooksExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public async Task SearchByTitleAsync_WithValidTitleFragment_ShouldReturnMatchingBooks()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);
            // Act
            var result = await _bookManager.SearchByTitleAsync("War and Peace");
            // Assert
            Assert.Equal("War and Peace", result.FirstOrDefault().Title);
        }

        [Fact]
        public async Task SearchByTitleAsync_WithInvalidTitleFragment_ShouldThrowKeyNotFoundException()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public async Task GetSpecificAsync_WithValidIsbn_ShouldReturnBook()
        {
            // Arrange
            await DatabaseSeeder.SeedDatabaseAsync(_dbContext, _bookManager);
            // Act
            var result = await _bookManager.GetSpecificAsync("1234567890123");
            // Assert
            Assert.Equal("1234567890123", result.ISBN);
        }

        [Fact]
        public async Task GetSpecificAsync_WithInvalidIsbn_ShouldThrowKeyNotFoundException()
        {
            // Arrange

            // Act

            // Assert
        }

        [Fact]
        public async Task UpdateAsync_WithValidBook_ShouldUpdateBook()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Some book title",
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            await _bookManager.AddAsync(newBook);
            await _bookManager.UpdateAsync(newBook);

            // Act

            newBook.Title = "Updated Title";

            // Assert
            var bookInDb = _dbContext.Books.FirstOrDefault(b => b.Title == newBook.Title);
            Assert.Equal("Updated Title", bookInDb.Title);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidBook_ShouldThrowValidationException()
        {
            // Arrange
            var newBook = new Book
            {
                Title = ,
                Author = "John Doe",
                ISBN = "1234567890123",
                YearPublished = 2021,
                Genre = "Fiction",
                Pages = 100,
                Price = 19.99
            };
            await _bookManager.AddAsync(newBook);
            await _bookManager.UpdateAsync(newBook);

            // Act

            // Assert
        }

    }
}
