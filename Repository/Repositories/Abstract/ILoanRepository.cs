using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Loan repository interface, implemented by <see cref="LoanRepository"/>
    /// </summary>
    public interface ILoanRepository
        : IBaseRepository<Loan>
    {
        Task<Loan?> GetLoanWithDetailsAsync(int id);
    }
}
