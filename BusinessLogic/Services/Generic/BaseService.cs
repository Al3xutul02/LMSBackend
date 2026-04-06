using AutoMapper;
using Repository.Enums.Behaviors;
using Repository.Repositories.Base;
using System.Linq.Expressions;
using System.Reflection;

namespace BusinessLogic.Services.Generic
{
    public class BaseService<T, TReadDto, TCreateDto, TUpdateDto>(
        IMapper mapper,
        IBaseRepository<T> repository) : IBaseService<T, TReadDto, TCreateDto, TUpdateDto>
        where T : class
        where TReadDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IMapper _mapper = mapper;
        protected readonly IBaseRepository<T> _repository = repository;

        public virtual async Task<TReadDto?> GetByIdAsync(int id, IncludeBehavior behavior, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id, behavior, includes);
                return entity != null ? _mapper.Map<TReadDto>(entity)
                    : throw new Exception("User not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public virtual async Task<IEnumerable<TReadDto>> GetAllAsync(IncludeBehavior behavior, params Expression<Func<T, object>>[] includes)
        {
            IEnumerable<T> entities = await _repository.GetAllAsync(behavior, includes);
            return _mapper.Map<IEnumerable<TReadDto>>(entities);
        }

        public virtual async Task<bool> CreateAsync(TCreateDto entityCreateDto)
        {
            try
            {
                var entity = _mapper.Map<T>(entityCreateDto);
                await _repository.AddAsync(entity);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id, IncludeBehavior.NoInclude)
                    ?? throw new Exception("User not found");
                _repository.Delete(entity);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }

        // Type cache for better performance (reflection is often slow)
        protected static readonly PropertyInfo? _idUpdateProperty = typeof(TUpdateDto).GetProperty("Id");
        public virtual async Task<bool> UpdateAsync(TUpdateDto entityUpdateDto)
        {
            var idValue = _idUpdateProperty?.GetValue(entityUpdateDto)
                ?? throw new ArgumentException($"DTO {typeof(TUpdateDto).Name} must have an 'Id' property.");

            int id = (int)idValue;

            var existing = await _repository.GetByIdAsync(id, IncludeBehavior.NoInclude);
            if (existing == null) return false;

            _mapper.Map(entityUpdateDto, existing);
            await _repository.SaveAsync();
            return true;
        }
    }
}
