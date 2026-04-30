using AutoMapper;
using Repository.Enums.Behaviors;
using Repository.Repositories.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace BusinessLogic.Services.Generic
{
    /// <summary>
    /// The implementation of the <see cref="IBaseService{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The class model for the repository</typeparam>
    /// <typeparam name="TReadDto">The read DTO of the entity</typeparam>
    /// <typeparam name="TCreateDto">The create DTO of the entity</typeparam>
    /// <typeparam name="TUpdateDto">The update DTO of the entity</typeparam>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="repository">The main repository the service communicates with</param>
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

        // Collection of valid primary keys in the database
        protected static readonly ISet<string> _validKeys = new HashSet<string>
        {
            "Id", "ISBN"
        };

        public virtual async Task<TReadDto?> GetByIdAsync(int id, IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null)
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

        public virtual async Task<IEnumerable<TReadDto>> GetAllAsync(IncludeBehavior behavior, Func<IQueryable<T>, IQueryable<T>>? includes = null)
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

        // Type cache for better performance (reflection is often slow)
        protected static readonly IQueryable<PropertyInfo> _entityProperties = typeof(TUpdateDto).GetProperties().AsQueryable();
        protected static readonly PropertyInfo? _key = _entityProperties.FirstOrDefault(p => _validKeys.Contains(p.Name));
        public virtual async Task<bool> UpdateAsync(TUpdateDto entityUpdateDto)
        {

            var idValue = _key?.GetValue(entityUpdateDto)
                ?? throw new ArgumentException($"DTO {typeof(TUpdateDto).Name} must have a primary key property.");

            int id = (int)idValue;

            var existing = await _repository.GetByIdAsync(id, IncludeBehavior.AllIncludes);
            if (existing == null) return false;

            _mapper.Map(entityUpdateDto, existing);
            await _repository.SaveAsync();
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
    }
}
