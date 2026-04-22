using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Book service interface, implemented by <see cref="BookService"/>
    /// </summary>
    public interface IBookService
        : IBaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>
    { }
}
