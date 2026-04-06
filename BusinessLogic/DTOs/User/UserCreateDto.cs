using Repository.Enums.Types;

namespace BusinessLogic.DTOs.User
{
    public record UserCreateDto(
        string Name = "",
        string Email = "",
        string Password = "",
        UserRole Role = UserRole.Reader
        );
}
