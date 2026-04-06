using Microsoft.EntityFrameworkCore;
using Repository.Enums.Behaviors;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace Repository.Builders
{
    public class QueryBuilder<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly Dictionary<IncludeBehavior, Func<IQueryable<T>>> behaviorMap;

        private Expression<Func<T, object>>[]? _includes;
        private IncludeBehavior _behavior = IncludeBehavior.NoInclude;

        public QueryBuilder(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
            behaviorMap = new Dictionary<IncludeBehavior, Func<IQueryable<T>>>
            {
                { IncludeBehavior.NoInclude, BehaviorMapNoIncludes },
                { IncludeBehavior.AllIncludes, BehaviorMapAllIncludes },
                { IncludeBehavior.SelectedIncludes, BehaviorMapSelectedIncludes }
            };
        }

        public QueryBuilder<T> AddIncludes(Expression<Func<T, object>>[] includes)
        {
            _includes = includes;
            return this;
        }

        public QueryBuilder<T> AddBehavior(IncludeBehavior behavior)
        {
            _behavior = behavior;
            return this;
        }

        public IQueryable<T> Build()
        {
            if (_includes == null)
            {
                return BehaviorMapNoIncludes();
            }

            return behaviorMap[_behavior]();
        }

        private IQueryable<T> BehaviorMapNoIncludes() => _dbSet;

        private IQueryable<T> BehaviorMapAllIncludes()
        {
            IQueryable<T> query = _dbSet;
            var properties = typeof(T).GetProperties()
                .Where(p =>
                    !p.IsDefined(typeof(NotMappedAttribute), true) &&
                    ((p.PropertyType.IsClass && p.PropertyType != typeof(string)) ||
                    (typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string)))
                );

            foreach (var property in properties)
            {
                query = query.Include(property.Name);
            }
            return query;
        }

        private IQueryable<T> BehaviorMapSelectedIncludes()
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in _includes!)
            {
                query = query.Include(include);
            }
            return query;
        }
    }
}
