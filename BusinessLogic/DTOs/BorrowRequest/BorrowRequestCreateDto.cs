namespace BusinessLogic.DTOs.BorrowRequest
{
    public record BorrowRequestCreateDto(
        int UserId = 0,
        int BookISBN = 0,
        int Count = 1
    );
}