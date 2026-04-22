using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Fine
{
    /// <summary>
    /// Represents a read DTO for fine entities.
    /// </summary>
    public record FineReadDto(
        int Id = 0,
        int LoanId = 0,
        int Amount = 0,
        FineStatus Status = FineStatus.Unpaid
        );
}
