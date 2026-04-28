using AutoMapper;
using BusinessLogic.DTOs.BorrowRequest;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class BorrowRequestService(
        IMapper mapper,
        IBorrowRequestRepository borrowRequestRepository,
        ILoanRepository loanRepository,
        ILoanBookRelationRepository loanBookRelationRepository,
        IBookRepository bookRepository)
        : BaseService<BorrowRequest, BorrowRequestReadDto, BorrowRequestCreateDto, BorrowRequestUpdateDto>(mapper, borrowRequestRepository),
          IBorrowRequestService
    {
        private readonly IBorrowRequestRepository _borrowRequestRepository = borrowRequestRepository;
        private readonly ILoanRepository _loanRepository = loanRepository;
        private readonly ILoanBookRelationRepository _loanBookRelationRepository = loanBookRelationRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        public override async Task<bool> CreateAsync(BorrowRequestCreateDto dto)
        {
            var entity = new BorrowRequest
            {
                UserId = dto.UserId,
                BookISBN = dto.BookISBN,
                Count = dto.Count,
                RequestDate = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };
            await _borrowRequestRepository.AddAsync(entity);
            await _borrowRequestRepository.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<BorrowRequestReadDto>> GetByStatusAsync(RequestStatus status)
        {
            var requests = await _borrowRequestRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<BorrowRequestReadDto>>(requests);
        }

        public async Task<bool> FinishAsync(int requestId)
        {
            var request = await _borrowRequestRepository.GetByIdAsync(requestId, IncludeBehavior.NoInclude);
            if (request == null || request.Status != RequestStatus.Pending) return false;

            var book = await _bookRepository.GetByIdAsync(request.BookISBN, IncludeBehavior.NoInclude);
            if (book == null || book.Count < request.Count) return false;

            var loan = new Loan
            {
                UserId = request.UserId,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Status = LoanStatus.Active
            };
            await _loanRepository.AddAsync(loan);
            await _loanRepository.SaveAsync(); // flush to get loan.Id

            await _loanBookRelationRepository.AddAsync(new LoanBookRelation
            {
                LoanId = loan.Id,
                BookISBN = request.BookISBN,
                Count = request.Count
            });

            book.Count -= request.Count;
            if (book.Count == 0) book.Status = BookStatus.OutOfStock;
            _bookRepository.Update(book);

            request.Status = RequestStatus.Approved;
            _borrowRequestRepository.Update(request);

            await _borrowRequestRepository.SaveAsync();
            return true;
        }

        public async Task<bool> RejectAsync(int requestId)
        {
            var request = await _borrowRequestRepository.GetByIdAsync(requestId, IncludeBehavior.NoInclude);
            if (request == null || request.Status != RequestStatus.Pending) return false;

            request.Status = RequestStatus.Rejected;
            _borrowRequestRepository.Update(request);
            await _borrowRequestRepository.SaveAsync();
            return true;
        }
    }
}