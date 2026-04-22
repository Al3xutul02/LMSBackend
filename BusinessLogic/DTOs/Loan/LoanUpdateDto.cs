using BusinessLogic.DTOs.Book;
using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Loan
{
    /// <summary>
    /// Represents an update DTO for loan entities.
    /// </summary>
    public record LoanUpdateDto(
        int Id = 0,
        DateTime IssueDate = new DateTime(),
        DateTime DueDate = new DateTime(),
        LoanStatus Status = LoanStatus.Active,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
