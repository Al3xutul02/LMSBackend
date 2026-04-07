using Microsoft.EntityFrameworkCore;
using Repository.Builders;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Repositories.Base;
using Repository.Tables;
using System.Linq.Expressions;

namespace Repository.Repositories
{
    public class BookRepository(DatabaseContext context)
        : BaseRepository<Book>(context), IBookRepository
    {
        public async Task<IEnumerable<Book>> GetAllWithFiltersAsync(string? title, string? author, int? branchId,
            IncludeBehavior behavior, params Expression<Func<Book, object>>[] includes)
        {
            IQueryable<Book> query = new QueryBuilder<Book>(_dbSet)
                .AddIncludes(includes)
                .AddBehavior(behavior)
                .Build();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(b => b.Author.Contains(author));
            }

            if (branchId != null)
            {
                query = query.Where(b => b.Branches != null && b.Branches.Any(br => br.BranchId == branchId));
            }

            return await query.ToListAsync();
        }
    }
}
