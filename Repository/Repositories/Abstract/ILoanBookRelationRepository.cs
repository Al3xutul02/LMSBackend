using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// LoanBookRelation repository interface, implemented by <see cref="LoanBookRelationRepository"/>
    /// </summary>
    public interface ILoanBookRelationRepository
        : IBaseRepository<LoanBookRelation>
    { }
}
