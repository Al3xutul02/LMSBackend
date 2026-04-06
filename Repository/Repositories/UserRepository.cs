using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Repositories.Abstract;
using Repository.Repositories.Base;
using Repository.Tables;

namespace Repository.Repositories
{
    public class UserRepository(DatabaseContext context)
        : BaseRepository<User>(context), IUserRepository
    {
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
        }
    }
}
