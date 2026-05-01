using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Loan service interface, implemented by <see cref="LoanService"/>
    /// </summary>
    public interface ILoanService
        : IBaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>
    {
        /// <summary>
        ///Creates a reservation for a user, if the user is not an "Unwanted Customer" (has unpaid fines).
        ///The reservation is created with the "Reserved" status, and the pickup date is set to the one provided by the UI.
        ///The method also creates the necessary entries in the LoanBookRelation table based on the provided DTO.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userId"></param>
        /// <param name="pickupDate"></param>
        /// <returns></returns>
        Task<bool> CreateReservationAsync(LoanCreateDto dto, int userId, DateTime pickupDate);
        /// <summary>
        ///  Asynchronously retrieves all active loan reservations.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
        /// cref="LoanReadDto"/> objects representing the active reservations. The collection is empty if there are no
        /// active reservations.</returns>
        Task<IEnumerable<LoanReadDto>> GetActiveReservationsAsync();
        /// <summary>
        /// Approves a reservation and activates the corresponding loan.
        /// </summary>
        /// <param name="id">The ID of the loan to approve and activate.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the <see cref="LoanReadDto"/> object representing the approved and activated loan.
        /// </returns>
        Task<LoanReadDto> ApproveAndActivateLoanAsync(int id);
    }
}
