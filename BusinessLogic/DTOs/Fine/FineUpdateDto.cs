using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Fine
{
    /// <summary>
    /// Represents an update DTO for fine entities.
    /// </summary>
    public record FineUpdateDto(
        int Id = 0,
        int LoanId = 0,
        int Amount = 0,
        FineStatus Status = FineStatus.Unpaid
        );
}
