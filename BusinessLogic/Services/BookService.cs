using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
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
    }
}
