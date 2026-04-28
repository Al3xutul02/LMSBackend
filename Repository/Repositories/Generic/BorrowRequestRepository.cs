using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories
{
    public class BorrowRequestRepository(DatabaseContext context)
        : BaseRepository<BorrowRequest>(context), IBorrowRequestRepository
    {
        public async Task<IEnumerable<BorrowRequest>> GetByStatusAsync(RequestStatus status)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.Status == status)
                .ToListAsync();
        }
    }
}