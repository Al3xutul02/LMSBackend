using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IBookService
        : IBaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>
    {
        Task<BookReadDto?> GetBookDetailsAsync(int isbn);
    }
}
