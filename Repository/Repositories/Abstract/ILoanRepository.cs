using Repository.Enums.Types;
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

        /// <summary>
        ///  Asynchronously retrieves all loans that match the specified status.
        /// </summary>
        /// <param name="status">The status of the loans to retrieve. Only loans with this status are included in the result.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of loans with the
        /// specified status. The collection is empty if no loans match the status.</returns>
        Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status);
        /// <summary>
        ///     Asynchronously determines whether the specified user has any unpaid fines.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for unpaid fines. Must be a non-negative integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// user has unpaid fines; otherwise, <see langword="false"/>.</returns>
        Task<bool> HasUnpaidFinesAsync(int userId);
    }
}
