using Microsoft.EntityFrameworkCore;
using Repository.Builders;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Repositories.Generic;
using Repository.Tables;
using System.Linq.Expressions;

namespace Repository.Repositories
{
    /// <summary>
    /// The implementation of the <see cref="IBookRepository"/> interface
    /// </summary>
    /// <param name="context">The context of the database that the repository belongs to</param>
    public class BookRepository(DatabaseContext context)
        : BaseRepository<Book>(context), IBookRepository
    {
        public async Task<IEnumerable<Book>> GetAllWithFiltersAsync(string? title, string? author, int? branchId)
        {
            IQueryable<Book> query = new QueryBuilder<Book>(_dbSet)
                .AddIncludes(query =>
                    query.Include(b => b.Genres)
                        .Include(b => b.Branches).ThenInclude(br => br.Branch)
                )
                .AddBehavior(IncludeBehavior.GivenIncludes)
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

        // În BookRepository.cs
        public async Task<IEnumerable<Book>> GetByGenresAsync(List<int> genres)
        {
            return await _dbSet
                .Include(b => b.Genres)
                .Include(b => b.Branches).ThenInclude(br => br.Branch)
                .Where(b => b.Genres.Any(g => genres.Contains((int)g.Genre)))
                .Take(4)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetRandomAsync(int count)
        {
            return await _dbSet
                .Include(b => b.Genres)
                .Include(b => b.Branches).ThenInclude(br => br.Branch)
                .OrderBy(b => Guid.NewGuid()) // random order
                .Take(count)
                .ToListAsync();
        }
    }
}
