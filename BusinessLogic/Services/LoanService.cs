using AutoMapper;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class LoanService : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>, ILoanService
    {
        private readonly ILoanBookRelationRepository _loanBookRelationRepository;
        private readonly IInventoryService _inventoryService; // Adăugat pentru returnare
        private ILoanRepository LoanRepository => (ILoanRepository)_repository;

        // Constructorul actualizat cu IInventoryService
        public LoanService(
            IMapper mapper,
            ILoanRepository loanRepository,
            ILoanBookRelationRepository loanBookRelationRepository,
            IInventoryService inventoryService)
            : base(mapper, loanRepository)
        {
            _loanBookRelationRepository = loanBookRelationRepository;
            _inventoryService = inventoryService;
        }

        public async Task<bool> CreateReservationAsync(LoanCreateDto dto, int userId, DateTime pickupDate)
        {
            if (await LoanRepository.HasUnpaidFinesAsync(userId))
            {
                throw new Exception("Rezervare respinsă: Utilizatorul are amenzi neplătite.");
            }

            var entity = _mapper.Map<Loan>(dto);
            entity.UserId = userId;
            entity.Status = LoanStatus.Pending;
            entity.IssueDate = DateTime.UtcNow;
            entity.DueDate = pickupDate;

            await LoanRepository.AddAsync(entity);
            await LoanRepository.SaveAsync();

            if (dto.BookRelations != null)
            {
                foreach (var relation in dto.BookRelations)
                {
                    var loanBookRelation = new LoanBookRelation
                    {
                        LoanId = entity.Id,
                        BookISBN = relation.ISBN,
                        Count = relation.Count
                    };
                    await _loanBookRelationRepository.AddAsync(loanBookRelation);
                }
            }

            await LoanRepository.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<LoanReadDto>> GetActiveReservationsAsync()
        {
            var reservations = await LoanRepository.GetByStatusAsync(LoanStatus.Active);
            return _mapper.Map<IEnumerable<LoanReadDto>>(reservations);
        }

        public async Task<IEnumerable<LoanReadDto>> GetBooksToReturnAsync()
        {
            var allLoans = await LoanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
            var toReturn = allLoans.Where(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue);
            return _mapper.Map<IEnumerable<LoanReadDto>>(toReturn);
        }

        public async Task<LoanReadDto> ApproveAndActivateLoanAsync(int id)
        {
            var loan = await LoanRepository.GetByIdAsync(id, IncludeBehavior.AllIncludes);

            if (loan == null)
                throw new Exception("Rezervarea nu a fost găsită.");

            loan.Status = LoanStatus.Active;
            loan.IssueDate = DateTime.UtcNow;
            loan.DueDate = DateTime.UtcNow.AddDays(14);

            LoanRepository.Update(loan);
            await LoanRepository.SaveAsync();

            return _mapper.Map<LoanReadDto>(loan);
        }

        public async Task<IEnumerable<LoanReadDto>> GetLoansByUserIdAsync(int userId)
        {
            var userLoans = await LoanRepository.GetLoansByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<LoanReadDto>>(userLoans);
        }

        public async Task<bool> MarkAsReturnedAsync(int loanId)
        {
            // Preluăm împrumutul cu cărțile incluse
            var loan = await LoanRepository.GetByIdAsync(loanId, IncludeBehavior.AllIncludes);

            if (loan == null || loan.Status == LoanStatus.Returned)
                return false;

            // 1. Marcare status în baza de date
            loan.Status = LoanStatus.Returned;
            loan.ReturnDate = DateTime.Now;

            // 2. Actualizare stoc în InventoryService
            // Verificăm dacă există relația cu cărțile (tabela de legătură)
            if (loan.Books != null && loan.Books.Any())
            {
                foreach (var relation in loan.Books)
                {
                    // Trimitem înapoi ISBN-ul și numărul de exemplare către inventarul filialei
                    await _inventoryService.AddBooksAsync(loan.BranchId, relation.BookISBN, relation.Count);
                }
            }

            LoanRepository.Update(loan);
            await LoanRepository.SaveAsync();

            return true;
        }
    }
}