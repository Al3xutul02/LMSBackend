namespace BusinessLogic.DTOs.Fine
{
    public record FineCreateDto(
        int LoanId = 0,
        int Amount = 0
        );
}
