using Repository.Enums.Behaviors;
using System.Linq.Expressions;

namespace Repository.Repositories.Base
{

    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task<IEnumerable<T>> GetAllAsync(IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }
}

