using AutoMapper;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
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
    }
}
