using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Repository.Enums.Behaviors;
using Repository.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Book service interface, implemented by <see cref="BookService"/>
    /// </summary>
    public interface IBookService
        : IBaseService<Book, BookReadDto, BookCreateDto, BookUpdateDto>
    {
        ///<summary>
        ///This task defines the IBookService interface within the BusinessLogic.Services.
        ///Abstract namespace.It serves as the primary abstraction 
        ///layer for book-related business logic, bridging the gap between the controllers 
        ///and the data repositories.
        ///</summary>
        Task<IEnumerable<BookReadDto>> GetAllWithFiltersAsync(string? title, string? author, int? branchId);

        /// <summary>
        /// Get book details by ISBN
        /// </summary>
        /// <param name="isbn">ISBN of the book</param>
        /// <returns>Book details if found, otherwise null</returns>
        Task<BookReadDto?> GetBookDetailsAsync(int isbn);
    }
}
