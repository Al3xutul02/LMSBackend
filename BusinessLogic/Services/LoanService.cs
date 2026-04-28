using AutoMapper;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    /// <summary>
    /// The implementation of the <see cref="ILoanService"/> interface
    /// </summary>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="loanRepository">The loan repository the service communicates with</param>
    public class LoanService(IMapper mapper, ILoanRepository loanRepository)
        : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>(mapper, loanRepository), ILoanService
    {
        private ILoanRepository LoanRepository => (ILoanRepository)_repository;

        public async Task<IEnumerable<LoanReadDto>> GetUserLoansAsync(int userId)
        {
            var loans = await LoanRepository.GetUserLoansAsync(userId);
            return _mapper.Map<IEnumerable<LoanReadDto>>(loans);
        }
    }
}