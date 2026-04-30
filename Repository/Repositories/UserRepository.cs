using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories
{
    /// <summary>
    /// The implementation of the <see cref="IUserRepository"/> interface
    /// </summary>
    /// <param name="context">The context of the database that the repository belongs to</param>
    public class UserRepository(DatabaseContext context)
        : BaseRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        }

        public async Task<int> GetActiveLoansCountAsync(int userId)
        {
            return await _context.Loans
                .CountAsync(l => l.UserId == userId && l.Status == LoanStatus.Active);
        }
    }
}
