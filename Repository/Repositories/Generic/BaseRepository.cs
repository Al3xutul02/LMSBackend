using Microsoft.EntityFrameworkCore;
using Repository.Builders;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using System.Linq.Expressions;

namespace Repository.Repositories.Base
{
    public class BaseRepository<T>(DatabaseContext context)
        : IBaseRepository<T> where T : class
    {
        protected readonly DatabaseContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(int id, IncludeBehavior behavior, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = new QueryBuilder<T>(_dbSet)
                .AddIncludes(includes)
                .AddBehavior(behavior)
                .Build();
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(IncludeBehavior behavior, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = new QueryBuilder<T>(_dbSet)
                .AddIncludes(includes)
                .AddBehavior(behavior)
                .Build();
            return await query.ToListAsync();
        }

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public virtual void Update(T entity) => _dbSet.Update(entity);

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
