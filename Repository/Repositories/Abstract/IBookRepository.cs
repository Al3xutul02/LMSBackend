using Repository.Enums.Behaviors;
using Repository.Repositories.Generic;
using Repository.Tables;
using System.Linq.Expressions;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Book repository interface, implemented by <see cref="BookRepository"/>
    /// </summary>
    public interface IBookRepository
        : IBaseRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllWithFiltersAsync(string? title, string? author, int? branchId);
    }
}
