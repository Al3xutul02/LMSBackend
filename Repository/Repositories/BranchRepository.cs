using Repository.Contexts;
using Repository.Repositories.Abstract;
using Repository.Repositories.Base;
using Repository.Tables;

namespace Repository.Repositories
{
    public class BranchRepository(DatabaseContext context)
        : BaseRepository<Branch>(context), IBranchRepository
    { }
}
