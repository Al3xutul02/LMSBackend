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
    public class LoanService(IMapper mapper, ILoanRepository loanRepository, IBookRepository bookRepository)
        : BaseService<Loan, LoanReadDto, LoanCreateDto, LoanUpdateDto>(mapper, loanRepository), ILoanService
    {
        private readonly IBookRepository _bookRepository = bookRepository;

        public override async Task<bool> UpdateAsync(LoanUpdateDto dto)
        {
            if (dto.Status == LoanStatus.Returned)
            {
                var existingLoan = await _repository.GetByIdAsync(dto.Id, IncludeBehavior.NoInclude, null);

                if (existingLoan != null && existingLoan.Status != LoanStatus.Returned)
                {
                    // Update book stock counts for each returned book
                    if (dto.BookRelations != null)
                    {
                        foreach (var relation in dto.BookRelations)
                        {
                            var book = await _bookRepository.GetByIdAsync(relation.ISBN, IncludeBehavior.NoInclude, null);

                            if (book != null)
                            {
                                book.Count += relation.Count;

                                if (book.Count > 0 && book.Status == BookStatus.OutOfStock)
                                    book.Status = BookStatus.InStock;

                                _bookRepository.Update(book);
                            }
                        }
                        await _bookRepository.SaveAsync();
                    }

                    // Update the already-tracked entity directly to avoid EF Core tracking conflict
                    existingLoan.Status = LoanStatus.Returned;
                    existingLoan.IssueDate = dto.IssueDate;
                    existingLoan.DueDate = dto.DueDate;
                    await _repository.SaveAsync();
                    return true;
                }
            }

            return await base.UpdateAsync(dto);
        }
    }
}
