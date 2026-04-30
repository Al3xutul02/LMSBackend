using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
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
        public async Task<Loan?> GetLoanWithDetailsAsync(int id)
        {
            return await _context.Loans
                .Include(l => l.User) 
                .Include(l => l.Books) 
                    .ThenInclude(lbr => lbr.Book) 
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
