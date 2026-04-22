using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Loan
{
    /// <summary>
    /// Represents a create DTO for loan entities.
    /// </summary>
    public record LoanCreateDto(
        string LoanerName = "",
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
