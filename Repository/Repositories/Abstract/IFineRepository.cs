using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Fine repository interface, implemented by <see cref="FineRepository"/>
    /// </summary>
    public interface IFineRepository
        : IBaseRepository<Fine>
    { }
}
