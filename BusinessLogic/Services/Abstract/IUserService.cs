using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// User service interface, implemented by <see cref="UserService"/>
    /// </summary>
    public interface IUserService
        : IBaseService<User, UserReadDto, UserCreateDto, UserUpdateDto>
    {
        Task<UserReadDto?> GetUserProfileAsync(int userId);
    }
}
