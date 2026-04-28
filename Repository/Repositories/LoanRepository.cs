using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
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
        public async Task<IEnumerable<Loan>> GetUserLoansAsync(int userId)
        {
            return await _context.Loans
                .Where(l => l.UserId == userId && l.Status != LoanStatus.Returned)
                .Include(l => l.User)
                .Include(l => l.Books)
                .ToListAsync();
        }
    }
}