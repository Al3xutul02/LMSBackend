using Repository.Repositories.Generic;
using Repository.Tables;

namespace Repository.Repositories.Abstract
{
    /// <summary>
    /// User repository interface, implemented by <see cref="UserRepository"/>
    /// </summary>
    public interface IUserRepository
        : IBaseRepository<User>
    {
        /// <summary>
        /// See if a specific email is already used by an user
        /// </summary>
        /// <param name="email">Email that needs to be checked</param>
        /// <returns>A task with the final result of the query</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Read the entity with a specific username
        /// </summary>
        /// <param name="username">Name of the searched user</param>
        /// <returns>A task with the final result of the query</returns>
        Task<User?> GetByUsernameAsync(string username);
    }
}
