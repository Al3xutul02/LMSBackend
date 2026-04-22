using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// BranchBookRelation repository interface, implemented by <see cref="BranchBookRelationRepository"/>
    /// </summary>
    public interface IBranchBookRelationRepository
        : IBaseRepository<BranchBookRelation>
    { }
}
