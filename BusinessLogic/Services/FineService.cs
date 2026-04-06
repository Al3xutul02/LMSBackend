using AutoMapper;
using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class FineService(IMapper mapper, IFineRepository fineRepository)
        : BaseService<Fine, FineReadDto, FineCreateDto, FineUpdateDto>(mapper, fineRepository), IFineService
    {
        private IFineRepository FineRepository => (IFineRepository)_repository;
    }
}
