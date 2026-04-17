namespace BusinessLogic.DTOs.Fine
{
    /// <summary>
    /// Represents a create DTO for fine entities.
    /// </summary>
    public record FineCreateDto(
        int LoanId = 0,
        int Amount = 0
        );
}
