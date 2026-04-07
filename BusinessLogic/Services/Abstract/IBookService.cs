using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IBookService
        : IBaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>
    {
        Task<IEnumerable<BookReadDto>> GetAllWithFiltersAsync(string? title, string? author, int? branchId);
    }
}
