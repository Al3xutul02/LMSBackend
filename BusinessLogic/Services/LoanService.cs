using AutoMapper;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;
using Repository.Enums.Types;
using Repository.Enums.Behaviors;

namespace BusinessLogic.Services
{
    public class LoanService(
        IMapper mapper,
        ILoanRepository loanRepository,
        ILoanBookRelationRepository loanBookRelationRepository)
        : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>(mapper, loanRepository), ILoanService
    {
        // Accesăm repository-ul specific pentru a folosi noile metode (HasUnpaidFinesAsync, GetByStatusAsync)
        private ILoanRepository LoanRepository => (ILoanRepository)_repository;
        private readonly ILoanBookRelationRepository LoanBookRelationRepository = loanBookRelationRepository;

        public async Task<bool> CreateReservationAsync(LoanCreateDto dto, int userId, DateTime pickupDate)
        {
            // 1. Validare "Unwanted Customer" folosind logica mutată în Repository
            if (await LoanRepository.HasUnpaidFinesAsync(userId))
            {
                throw new Exception("Rezervare respinsă: Utilizatorul are amenzi neplătite.");
            }

            // 2. Mapare DTO -> Entity
            var entity = _mapper.Map<Loan>(dto);
            entity.UserId = userId;
            entity.Status = LoanStatus.Pending; // Folosim noul status pentru rezervări
            entity.IssueDate = DateTime.UtcNow;
            entity.DueDate = pickupDate; // Data de ridicare stabilită în UI

            // 3. Salvare folosind metodele din BaseRepository
            await LoanRepository.AddAsync(entity);
            await LoanRepository.SaveAsync();

            foreach (var relation in dto.BookRelations!)
            {
                var loanBookRelation = new LoanBookRelation
                {
                    LoanId = entity.Id,
                    BookISBN = relation.ISBN,
                    Count = relation.Count
                };
                await LoanBookRelationRepository.AddAsync(loanBookRelation);
            }

            await LoanRepository.SaveAsync();

            return true;
        }

        public async Task<IEnumerable<LoanReadDto>> GetActiveReservationsAsync()
        {
            // Preluăm doar rezervările (status Reserved) prin metoda specifică din Repository
            var reservations = await LoanRepository.GetByStatusAsync(LoanStatus.Active);

            return _mapper.Map<IEnumerable<LoanReadDto>>(reservations);
        }

        public async Task<LoanReadDto> ApproveAndActivateLoanAsync(int id)
        {
            // Preluăm entitatea cu toate includerile necesare
            var loan = await LoanRepository.GetByIdAsync(id, IncludeBehavior.AllIncludes);

            if (loan == null || loan.Status != LoanStatus.Active)
                throw new Exception("Rezervarea nu a fost găsită sau este deja activă.");

            // Transformăm rezervarea în împrumut activ
            loan.Status = LoanStatus.Active;
            loan.IssueDate = DateTime.UtcNow;
            loan.DueDate = DateTime.UtcNow.AddDays(14); // Termen standard de 2 săptămâni

            // Actualizare folosind BaseRepository
            LoanRepository.Update(loan);
            await LoanRepository.SaveAsync();

            return _mapper.Map<LoanReadDto>(loan);
        }

        public async Task<IEnumerable<LoanReadDto>> GetLoansByUserIdAsync(int userId)
        {
            var allLoans = await LoanRepository.GetAllAsync(IncludeBehavior.AllIncludes, null);
            var userLoans = allLoans.Where(l => l.UserId == userId).ToList();

            // Dacă vrei să numeri volumele totale:
            // var totalBooks = userLoans.Sum(l => l.BookRelations.Sum(br => br.Count));

            return _mapper.Map<IEnumerable<LoanReadDto>>(userLoans);
        }
    }
}