using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Loan
{
    public record LoanCreateDto(
        string LoanerName = "",
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
