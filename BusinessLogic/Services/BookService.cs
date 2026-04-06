using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class BookService(IMapper mapper, IBookRepository bookRepository)
        : BaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>(mapper, bookRepository), IBookService
    {
        private IBookRepository BookRepository => (IBookRepository)_repository;
    }
}
