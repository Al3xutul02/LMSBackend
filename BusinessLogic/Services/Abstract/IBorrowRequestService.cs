using BusinessLogic.DTOs.BorrowRequest;
using BusinessLogic.Services.Generic;
using Repository.Enums.Types;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IBorrowRequestService
        : IBaseService<BorrowRequest, BorrowRequestReadDto, BorrowRequestCreateDto, BorrowRequestUpdateDto>
    {
        Task<IEnumerable<BorrowRequestReadDto>> GetByStatusAsync(RequestStatus status);
        Task<bool> FinishAsync(int requestId);
        Task<bool> RejectAsync(int requestId);
    }
}