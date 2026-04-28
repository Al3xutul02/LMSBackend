using Repository.Enums.Types;
using Repository.Repositories.Base;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Loan repository interface, implemented by <see cref="LoanRepository"/>
    /// </summary>
    public interface ILoanRepository
        : IBaseRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status);
        Task<bool> HasUnpaidFinesAsync(int userId);
    }
}
