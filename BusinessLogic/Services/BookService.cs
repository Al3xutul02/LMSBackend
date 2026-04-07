using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Tables;
using System.Linq;

namespace BusinessLogic.Services
{
    public class BookService(IMapper mapper, IBookRepository bookRepository)
        : BaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>(mapper, bookRepository), IBookService
    {
        private IBookRepository BookRepository => (IBookRepository)_repository;

        public async Task<IEnumerable<BookReadDto>> GetAllWithFiltersAsync(
            string? title,
            string? author,
            int? branchId)
        {
            var books = await BookRepository.GetAllWithFiltersAsync(title, author, branchId, IncludeBehavior.SelectedIncludes, b => b.Genres, b => b.Branches);
            return _mapper.Map<IEnumerable<BookReadDto>>(books);
        }
    }
}