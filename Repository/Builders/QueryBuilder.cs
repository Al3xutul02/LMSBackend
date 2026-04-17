using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Repository.Enums.Behaviors;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Security.Policy;

namespace Repository.Builders
{
    public class QueryBuilder<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly Dictionary<IncludeBehavior, Func<IQueryable<T>>> behaviorMap;

        private Func<IQueryable<T>, IQueryable<T>>? _includes;
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

        public QueryBuilder<T> AddIncludes(Func<IQueryable<T>, IQueryable<T>>? includes)
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
            if (_includes != null)
            {
                query = _includes(query);
            }
            return query;
        }
    }
}
