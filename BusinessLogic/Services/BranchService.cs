using AutoMapper;
using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    /// <summary>
    /// The implementation of the <see cref="IBranchService"/> interface
    /// </summary>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="branchRepository">The branch repository the service communicates with</param>
    public class BranchService(IMapper mapper, IBranchRepository branchRepository)
        : BaseService<Branch, BranchReadDto, BranchCreateDto, BranchUpdateDto>(mapper, branchRepository), IBranchService
    {
        private IBranchRepository BranchRepository => (IBranchRepository)_repository;
    }
}
