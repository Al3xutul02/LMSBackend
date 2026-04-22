using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Branch repository interface, implemented by <see cref="BranchRepository"/>
    /// </summary>
    public interface IBranchRepository
        : IBaseRepository<Branch>
    { }
}
