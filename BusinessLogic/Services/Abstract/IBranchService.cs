using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Branch service interface, implemented by <see cref="BranchService"/>
    /// </summary>
    public interface IBranchService
        : IBaseService<Branch, BranchReadDto, BranchCreateDto, BranchUpdateDto>
    { }
}
