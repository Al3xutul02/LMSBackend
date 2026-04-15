using Microsoft.EntityFrameworkCore;
using Repository.Builders;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using System.Linq.Expressions;
using System.Reflection;

namespace Repository.Repositories.Base
{
    public class BaseRepository<T>(DatabaseContext context)
        : IBaseRepository<T> where T : class
    {
        protected readonly DatabaseContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        // Collection of valid primary keys in the database
        protected static readonly ISet<string> _validKeys = new HashSet<string>
        {
            "Id", "ISBN"
        };

        // Type cache for better performance (reflection is often slow)
        protected static readonly IQueryable<PropertyInfo> _entityProperties = typeof(T).GetProperties().AsQueryable();
        protected static readonly string _keyName = _entityProperties.First(p => _validKeys.Contains(p.Name)).Name;
        public virtual async Task<T?> GetByIdAsync(int id, IncludeBehavior behavior, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = new QueryBuilder<T>(_dbSet)
                .AddIncludes(includes)
                .AddBehavior(behavior)
                .Build();

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, _keyName) == id);
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
