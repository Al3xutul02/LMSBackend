using Repository.Contexts;
using Repository.Repositories.Abstract;
using Repository.Repositories.Base;
using Repository.Tables;

namespace Repository.Repositories
{
    public class BookGenreRepository(DatabaseContext context)
        : BaseRepository<BookGenre>(context), IBookGenreRepository
    { }
}
