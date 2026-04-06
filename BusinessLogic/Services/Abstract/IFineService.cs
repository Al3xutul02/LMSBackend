using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IFineService
        : IBaseService<Fine, FineReadDto, FineCreateDto, FineUpdateDto>
    { }
}
