using AutoMapper;
using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    /// <summary>
    /// The implementation of the <see cref="IFineService"/> interface
    /// </summary>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="fineRepository">The fine repository the service communicates with</param>
    public class FineService(IMapper mapper, IFineRepository fineRepository)
        : BaseService<Fine, FineReadDto, FineCreateDto, FineUpdateDto>(mapper, fineRepository), IFineService
    {
        private IFineRepository FineRepository => (IFineRepository)_repository;
    }
}
