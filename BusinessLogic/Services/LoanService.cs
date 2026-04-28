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
                // REZOLVARE CS1501: Adăugăm 'null' pentru parametrul 'includes' care este obligatoriu în semnătură
                var existingLoan = await _repository.GetByIdAsync(dto.Id, IncludeBehavior.NoInclude, null);

                if (existingLoan != null && existingLoan.Status != LoanStatus.Returned)
                {
                    if (dto.BookRelations != null)
                    {
                        foreach (var relation in dto.BookRelations)
                        {
                            // REZOLVARE CS1501: Adăugăm 'null' și aici
                            var book = await _bookRepository.GetByIdAsync(relation.ISBN, IncludeBehavior.NoInclude, null);

                            if (book != null)
                            {
                                book.Count += relation.Count;

                                if (book.Count > 0 && book.Status == BookStatus.OutOfStock)
                                    book.Status = BookStatus.InStock;

                                // REZOLVARE CS1061: Folosim 'Update' (metoda void), NU 'UpdateAsync'
                                _bookRepository.Update(book);
                            }
                        }
                        // Salvăm modificările pentru stocul cărților
                        await _bookRepository.SaveAsync();
                    }
                    return await base.UpdateAsync(dto);
                }
            }

            // Apelăm base.UpdateAsync care se ocupă de salvarea statusului împrumutului
            return await base.UpdateAsync(dto);
        }
    }
}