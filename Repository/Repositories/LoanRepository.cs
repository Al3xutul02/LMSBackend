using Microsoft.EntityFrameworkCore;
using Repository.Builders;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories
{
    /// <summary>
    /// The implementation of the <see cref="ILoanRepository"/> interface
    /// </summary>
    /// <param name="context">The context of the database that the repository belongs to</param>
    public class LoanRepository(DatabaseContext context)
        : BaseRepository<Loan>(context), ILoanRepository
    {
        public async Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status)
        {
            // Păstrăm înălțimea de 70px în UI prin returnarea datelor corecte
            IQueryable<Loan> query = new QueryBuilder<Loan>(_dbSet)
                .AddIncludes(query => query.Include(l => l.Books))
                .AddBehavior(IncludeBehavior.GivenIncludes)
                .Build();
            return await query.Where(l => l.Status == status).ToListAsync();
        }

        public async Task<bool> HasUnpaidFinesAsync(int userId)
        {
            // Logica pentru "Unwanted Customer"
            return await _dbSet
                .AnyAsync(l => l.UserId == userId &&
                               l.Fine != null &&
                               l.Fine.Status == FineStatus.Unpaid);
        }
    }
}