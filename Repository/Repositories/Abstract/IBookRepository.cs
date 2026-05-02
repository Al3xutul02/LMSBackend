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
        //<summary>
        //This task involves implementing the GetAllWithFiltersAsync
        //method within the repository or service layer.
        //It is designed to perform flexible, asynchronous queries
        //against the database to retrieve a specific subset of book entities.
        Task<IEnumerable<Book>> GetAllWithFiltersAsync(string? title, string? author, int? branchId);

        Task<IEnumerable<Book>> GetByGenresAsync(List<int> genres);
        Task<IEnumerable<Book>> GetRandomAsync(int count);
    }
}
