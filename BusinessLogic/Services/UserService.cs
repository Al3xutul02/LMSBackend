using AutoMapper;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    /// <summary>
    /// The implementation of the <see cref="IUserService"/> interface
    /// </summary>
    /// <param name="mapper">The mapper for the DTOs and models</param>
    /// <param name="userRepository">The user repository the service communicates with</param>
    public class UserService(IMapper mapper, IUserRepository userRepository)
        : BaseService<User, UserReadDto, UserCreateDto, UserUpdateDto>(mapper, userRepository), IUserService
    {
        private IUserRepository UserRepository => (IUserRepository)_repository;

        public async Task<UserReadDto?> GetUserProfileAsync(int userId)
        {
            var user = await UserRepository.GetUserForProfileAsync(userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<bool> UpdateNameAsync(int id, string newName)
        {
            var user = await _repository.GetByIdAsync(id, IncludeBehavior.NoIncludes);
            if (user == null) return false;

            user.Name = newName;
            UserRepository.Update(user);
            await UserRepository.SaveAsync();
            return true;
        }

        public override async Task<bool> CreateAsync(UserCreateDto entityCreateDto)
        {
            try
            {
                var entity = _mapper.Map<User>(entityCreateDto);
                entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash, workFactor: 12);
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
    }
}
