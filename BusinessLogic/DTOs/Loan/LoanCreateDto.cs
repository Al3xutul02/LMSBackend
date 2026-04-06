using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Loan
{
    public record LoanCreateDto(
        string LoanerName = "",
        ICollection<BookRelationDto>? BookRelations = null
        );
}
