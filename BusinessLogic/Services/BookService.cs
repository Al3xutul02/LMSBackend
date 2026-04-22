using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Tables;

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

        public async Task<BookReadDto?> GetBookDetailsAsync(int isbn)
        {
            var book = await _repository.GetByIdAsync(isbn, IncludeBehavior.SelectedIncludes, b => b.Loans);
            if (book == null)
            {
                return null;
            }


            var loanedBooks = book.Loans.Sum(l => l.Count);

            var availableCopies = book.Count - loanedBooks;
            var systemLoanPeriod = 14;

            var dto = _mapper.Map<BookReadDto>(book);

            return dto with
            {
                LoanDurationDays = systemLoanPeriod,
                CanBeReserved = availableCopies > 0
            };
        }
    }
}
