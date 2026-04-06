using AutoMapper;
using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class BranchService(IMapper mapper, IBranchRepository branchRepository)
        : BaseService<Branch, BranchReadDto, BranchCreateDto, BranchUpdateDto>(mapper, branchRepository), IBranchService
    {
        private IBranchRepository BranchRepository => (IBranchRepository)_repository;
    }
}
