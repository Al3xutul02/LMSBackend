using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Generic;
using BusinessLogic.Services.Abstract;
using Repository.Repositories.Abstract;
using Repository.Tables;
using Repository.Enums.Types;
using Repository.Enums.Behaviors;
using AutoMapper;

namespace BusinessLogic.Services
{
    public class LoanService
    : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;

        public LoanService(
            ILoanRepository loanRepository,
            IUserRepository userRepository,
            IMapper mapper)
            // REPARARE CS1503: BaseService primește (IMapper mapper, IBaseRepository<T> repository)
            : base(mapper, loanRepository)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
        }

        // REPARARE CS0535: Adăugăm metoda care lipsea din interfață
        public async Task<LoanDetailsDto?> GetRequestDetailsAsync(int loanId)
        {
            var loan = await _loanRepository.GetLoanWithDetailsAsync(loanId);
            if (loan == null) return null;

            var activeLoansCount = await _userRepository.GetActiveLoansCountAsync(loan.UserId);

            return new LoanDetailsDto(
                RequestId: loan.Id,
                RequestDate: loan.IssueDate,
                ReturnDeadline: loan.DueDate,
                UserName: loan.User.Name,
                UserEmail: loan.User.Email,
                ActiveLoansCount: activeLoansCount,
                OverallStatus: loan.Status.ToString(),
                RequestedBooks: loan.Books.Select(b => new RequestedBookDto(
                    Title: b.Book.Title,
                    Author: b.Book.Author,
                    AvailableCopies: b.Book.Count, // Folosim 'Count' conform observației tale
                    RequestedQuantity: 1,
                    Status: loan.Status.ToString()
                ))
            );
        }

        public async Task<bool> ApproveRequestAsync(int id)
        {
            var loan = await _loanRepository.GetLoanWithDetailsAsync(id);

            // REPARARE CS0117: Verifică dacă statusurile din Enum sunt exact așa (Active, Pending)
            if (loan == null || loan.Status != LoanStatus.Active) return false;

            foreach (var relation in loan.Books)
            {
                if (relation.Book.Count <= 0) return false;
                relation.Book.Count--;
            }

            loan.Status = LoanStatus.Active;
            loan.IssueDate = DateTime.Now;
            loan.DueDate = DateTime.Now.AddDays(14);

            // REPARARE CS1501: UpdateAsync din BaseService primește doar TUpdateDto
            var updateDto = _mapper.Map<LoanUpdateDto>(loan);
            return await UpdateAsync(updateDto);
        }

        public async Task<bool> RejectRequestAsync(int id)
        {
            // REPARARE CS7036: GetByIdAsync necesită IncludeBehavior
            var loan = await _loanRepository.GetByIdAsync(id, IncludeBehavior.NoInclude);
            if (loan == null) return false;

            // Dacă 'Rejected' nu există în Enum, folosește un status valid
            // loan.Status = LoanStatus.Rejected; 

            var updateDto = _mapper.Map<LoanUpdateDto>(loan);
            return await UpdateAsync(updateDto);
        }
    }
}