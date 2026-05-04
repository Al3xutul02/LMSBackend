using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Microsoft.EntityFrameworkCore;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Tables;
using System.Linq;

namespace BusinessLogic.Services
{
    /// <summary>
    /// The implementation of the <see cref="IBookService"/> interface
    /// </summary>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="bookRepository">The book repository the service communicates with</param>
    public class BookService(IMapper mapper, IBookRepository bookRepository)
        : BaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>(mapper, bookRepository), IBookService
    {
        private IBookRepository BookRepository => (IBookRepository)_repository;

        public async Task<IEnumerable<BookReadDto>> GetAllWithFiltersAsync(
            string? title,
            string? author,
            int? branchId)
        {
            var books = await BookRepository.GetAllWithFiltersAsync(title, author, branchId);
            return _mapper.Map<IEnumerable<BookReadDto>>(books);
        }

        /// <summary>
        /// Retrieves the detailed information of a book by its ISBN, including related genres, loans, and branches.
        /// Calculates availability based on the current stock and active loans.
        /// </summary>
        /// <param name="isbn">The International Standard Book Number (ISBN) of the book.</param>
        /// <returns>
        /// A <see cref="BookReadDto"/> containing the book details, loan duration, and reservation status; 
        /// or <c>null</c> if no book is found with the provided ISBN.
        /// </returns>
        public async Task<BookReadDto?> GetBookDetailsAsync(int isbn)
        {
            var book = await _repository.GetByIdAsync(
                isbn,
                IncludeBehavior.AllIncludes,
                query => query
                    .Include(b => b.Genres)   // <-- asigură-te că e inclus
                    .Include(b => b.Loans)
                    .Include(b => b.Branches).ThenInclude(br => br.Branch)
            );

            if (book == null) return null;

            var loanedBooks = book.Loans.Sum(l => l.Count);
            var availableCopies = book.Count - loanedBooks;

            var dto = _mapper.Map<BookReadDto>(book);
            return dto with
            {
                LoanDurationDays = 14,
                CanBeReserved = availableCopies > 0
            };
        }

        /// <summary>
        /// Retrieves a collection of books that belong to any of the specified genres.
        /// </summary>
        /// <param name="genres">A list of genre identifiers used to filter the books.</param>
        /// <returns>
        /// A collection of <see cref="BookReadDto"/> representing the books found for the given genres.
        /// </returns>
        public async Task<IEnumerable<BookReadDto>> GetBooksByGenresAsync(List<int> genres)
        {
            var books = await BookRepository.GetByGenresAsync(genres);
            return _mapper.Map<IEnumerable<BookReadDto>>(books);
        }

        /// <summary>
        /// Retrieves a specified number of random books from the repository.
        /// Usually used for providing recommendations or filling empty search results.
        /// </summary>
        /// <param name="count">The number of random books to retrieve.</param>
        /// <returns>
        /// A collection of <see cref="BookReadDto"/> containing the randomly selected books.
        /// </returns>
        public async Task<IEnumerable<BookReadDto>> GetRandomBooksAsync(int count)
        {
            var books = await BookRepository.GetRandomAsync(count);
            return _mapper.Map<IEnumerable<BookReadDto>>(books);
        }
    }
}