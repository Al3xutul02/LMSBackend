using AutoMapper;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class LoanService(IMapper mapper, ILoanRepository loanRepository)
        : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>(mapper, loanRepository), ILoanService
    {
        private ILoanRepository LoanRepository => (ILoanRepository)_repository;
    }
}
