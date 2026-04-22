using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// Book repository interface, implemented by <see cref="BookRepository"/>
    /// </summary>
    public interface IBookRepository
        : IBaseRepository<Book>
    { }
}
