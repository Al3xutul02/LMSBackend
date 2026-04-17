using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// BookGenre repository interface, implemented by <see cref="BookGenreRepository"/>
    /// </summary>
    public interface IBookGenreRepository
        : IBaseRepository<BookGenre>
    { }
}
