using LibraryManagementSystem;

namespace LibraryManagementTest
{
    [TestFixture]
    public class LibraryTests
    {
        [Test]
        public void AddBook_ShouldAddANewBook_ToTheLibrary()
        {
            Library library = new Library();
            var newBook = new Book
            {
                Author = "Ivan",
                Id = 1,
                IsCheckedOut = false,
                Title = "Title",
            };
            library.AddBook(newBook);

            var allBooks = library.GetAllBooks();
            Assert.That(allBooks.Count(), Is.EqualTo(1));

            var singleBook = allBooks.First();
            Assert.That(singleBook.Id, Is.EqualTo(newBook.Id));
            Assert.That(singleBook.Title, Is.EqualTo(newBook.Title));
            Assert.That(singleBook.Author, Is.EqualTo(newBook.Author));
            Assert.IsFalse(singleBook.IsCheckedOut);
        }

        [Test]
        public void CheckedOutBook_ShouldReturnFalse_IfBookDoesNotExist()
        {
            Library library = new Library();
            var newBook = new Book
            {
                Author = "JK Rolling",
                Id = 1,
                IsCheckedOut = true,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.CheckOutBook(999);

            Assert.IsFalse(result);
        }
        [Test]
        public void CheckedOutBook_ShouldReturnFalse_IfBookIsAlreadyCheckedOut()
        {
            Library library = new Library();
            var newBook = new Book
            {
                Author = "John Doe",
                Id = 1,
                IsCheckedOut = true,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.CheckOutBook(newBook.Id);

            Assert.IsFalse(result);
        }
        [Test]
        public void CheckedOutBook_ShouldReturnTrue_IfBookExist()
        {
            Library library = new Library();
            var newBook = new Book
            {
                Author = "John Doe",
                Id = 1,
                IsCheckedOut = false,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.CheckOutBook(newBook.Id);

            Assert.IsTrue(result);
            var allBooks = library.GetAllBooks();
            var singleBook = allBooks.First();
            Assert.IsTrue(singleBook.IsCheckedOut);
        }
        [Test]
        public void ReturnBook_ShouldReturnFalse_IfBookDoesNotExsist()
        {
            var library = new Library();
            var newBook = new Book
            {
                Author = "JK Rolling",
                Id = 1,
                IsCheckedOut = true,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.ReturnBook(999);

            Assert.IsFalse(result);
        }
        [Test]
        public void ReturnBook_ShouldReturnFalse_IfBookIsNotCheckedOut()
        {
            var library = new Library();
            var newBook = new Book
            {
                Author = "JK Rolling",
                Id = 1,
                IsCheckedOut = false,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.ReturnBook(1);

            Assert.IsFalse(result);
        }
        [Test]
        public void ReturnBook_ShouldReturnTrue_IfBookCanBeCheckedOut()
        {
            var library = new Library();
            var newBook = new Book
            {
                Author = "JK Rolling",
                Id = 1,
                IsCheckedOut = true,
                Title = "Title",
            };
            library.AddBook(newBook);

            var result = library.ReturnBook(1);

            Assert.IsTrue(result);
            var allBooks = library.GetAllBooks();
            var singleBook = allBooks.First();
            Assert.IsFalse(singleBook.IsCheckedOut);
        }
    }
}
