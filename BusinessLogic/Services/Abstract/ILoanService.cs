using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface ILoanService
        : IBaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>
    { }
}
