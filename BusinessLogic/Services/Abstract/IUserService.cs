using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Generic;
using Repository.Tables;

namespace BusinessLogic.Services.Abstract
{
    public interface IUserService
        : IBaseService<User, UserReadDto, UserCreateDto, UserUpdateDto>
    { }
}
