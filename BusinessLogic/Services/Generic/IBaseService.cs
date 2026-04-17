using Repository.Enums.Behaviors;
using System.Linq.Expressions;

namespace BusinessLogic.Services.Generic
{
    public interface IBaseService<T, TReadDto, TCreateDto, TUpdateDto>
        where T : class
        where TReadDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TReadDto?> GetByIdAsync(int id, IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task<IEnumerable<TReadDto>> GetAllAsync(IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null);
        Task<bool> CreateAsync(TCreateDto entityCreateDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(TUpdateDto entityCreateDto);
    }
}
