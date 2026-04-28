using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    public interface IBorrowRequestRepository : IBaseRepository<BorrowRequest>
    {
        Task<IEnumerable<BorrowRequest>> GetByStatusAsync(Repository.Enums.Types.RequestStatus status);
    }
}