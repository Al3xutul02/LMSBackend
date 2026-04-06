using Repository.Repositories.Base;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    public interface IUserRepository
        : IBaseRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}
