using Repository.Enums.Types;

namespace BusinessLogic.DTOs.BorrowRequest
{
    public record BorrowRequestReadDto(
        int Id = 0,
        string RequesterName = "",
        string BookTitle = "",
        int BookISBN = 0,
        int Count = 1,
        DateTime RequestDate = new(),
        RequestStatus Status = RequestStatus.Pending
    );
}