using Repository.Enums.Behaviors;
using Repository.Repositories.Base;
using Repository.Tables;
using System.Linq.Expressions;

namespace Repository.Repositories.Abstract
{
    public interface IBookRepository
        : IBaseRepository<Book>
    {
        Task<IEnumerable<Book>> GetAllWithFiltersAsync(string? title, string? author, int? branchId,
            IncludeBehavior behavior, params Expression<Func<Book, object>>[] includes);
    }
}
