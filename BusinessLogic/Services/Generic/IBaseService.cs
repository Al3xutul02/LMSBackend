using Repository.Enums.Behaviors;
using System.Linq.Expressions;

namespace BusinessLogic.Services.Generic
{
    /// <summary>
    /// General service implementation that contains basic CRUD operations on the database
    /// </summary>
    /// <typeparam name="T">The class model for the service</typeparam>
    public interface IBaseService<T, TReadDto, TCreateDto, TUpdateDto>
        where T : class
        where TReadDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        /// <summary>
        /// Read the entity with a specific key
        /// </summary>
        /// <param name="id">The key of the entity</param>
        /// <param name="behavior">Behavior describing what joins the query should include</param>
        /// <param name="includes">Specific includes to be used</param>
        /// <returns>A task with the final result of the query</returns>
        Task<TReadDto?> GetByIdAsync(int id, IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);

        /// <summary>
        /// Read all entities from the table (WARNING: only use in testing with few entities in a table)
        /// </summary>
        /// <param name="behavior">Behavior describing what joins the query should include</param>
        /// <param name="includes">Specific includes to be used</param>
        /// <returns>A task with the final result of the query</returns>
        Task<IEnumerable<TReadDto>> GetAllAsync(IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);

        /// <summary>
        /// Create an entity in the table
        /// </summary>
        /// <param name="entityCreateDto">Entity DTO to be used for the creation</param>
        /// <returns>A task with confirmation of the action</returns>
        Task<bool> CreateAsync(TCreateDto entityCreateDto);

        /// <summary>
        /// Update an entity in the table
        /// </summary>
        /// <param name="entityUpdateDto">Entity DTO to be used for the update.
        /// It uses the primary key to find the entity in the table</param>
        /// <returns>A task with confirmation of the action</returns>
        Task<bool> UpdateAsync(TUpdateDto entityUpdateDto);

        /// <summary>
        /// Delete an entity in the table
        /// </summary>
        /// <param name="id">Primary key of the entity to be deleted</param>
        /// <returns>A task with confirmation of the action</returns>
        Task<bool> DeleteAsync(int id);
    }
}
