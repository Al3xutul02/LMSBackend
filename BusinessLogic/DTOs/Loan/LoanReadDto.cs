using BusinessLogic.DTOs.Book;
using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Loan
{
    /// <summary>
    /// Represents a read DTO for loan entities.
    /// </summary>
    public record LoanReadDto(
        int Id = 0,
        string LoanerName = "",
        int? FineId = null,
        DateTime IssueDate = new DateTime(),
        DateTime DueDate = new DateTime(),
        LoanStatus Status = LoanStatus.Active,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
