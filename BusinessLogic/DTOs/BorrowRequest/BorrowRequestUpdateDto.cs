using Repository.Enums.Types;

namespace BusinessLogic.DTOs.BorrowRequest
{
    public record BorrowRequestUpdateDto(
        int Id = 0,
        RequestStatus Status = RequestStatus.Pending
    );
}