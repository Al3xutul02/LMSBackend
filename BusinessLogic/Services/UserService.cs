using AutoMapper;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using BusinessLogic.Services.Generic;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class UserService(IMapper mapper, IUserRepository postRepository)
        : BaseService<User, UserReadDto, UserCreateDto, UserUpdateDto>(mapper, postRepository), IUserService
    {
        private IUserRepository UserRepository => (IUserRepository)_repository;
    }
}
