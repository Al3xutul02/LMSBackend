using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Fine service interface, implemented by <see cref="FineService"/>
    /// </summary>
    public interface IFineService
        : IBaseService<Fine, FineReadDto, FineCreateDto, FineUpdateDto>
    { }
}
