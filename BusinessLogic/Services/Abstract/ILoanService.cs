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
        // Metode specifice pentru fluxul de Librarian (User Story-ul tău)
        Task<LoanDetailsDto?> GetRequestDetailsAsync(int loanId);
        Task<bool> ApproveRequestAsync(int id);
        Task<bool> RejectRequestAsync(int id);
    }
}