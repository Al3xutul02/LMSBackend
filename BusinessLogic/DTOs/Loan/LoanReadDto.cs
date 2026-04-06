using BusinessLogic.DTOs.Book;
using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Loan
{
    public record LoanReadDto(
        int Id = 0,
        string LoanerName = "",
        int? FineId = null,
        DateTime IssueDate = new DateTime(),
        DateTime DueDate = new DateTime(),
        LoanStatus Status = LoanStatus.Active,
        ICollection<BookRelationDto>? BookRelations = null
        );
}
