using Repository.Contexts;
using Repository.Repositories.Abstract;
using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories
{
    /// <summary>
    /// The implementation of the <see cref="ILoanBookRelationRepository"/> interface
    /// </summary>
    /// <param name="context">The context of the database that the repository belongs to</param>
    public class LoanBookRelationRepository(DatabaseContext context)
        : BaseRepository<LoanBookRelation>(context), ILoanBookRelationRepository
    {
        
    }
}
