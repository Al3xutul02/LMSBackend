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
    { }
}
