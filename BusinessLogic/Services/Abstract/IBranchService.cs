using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IBranchService
        : IBaseService<Branch, BranchReadDto, BranchCreateDto, BranchUpdateDto>
    { }
}
