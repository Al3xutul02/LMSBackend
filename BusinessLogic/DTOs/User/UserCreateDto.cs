using Repository.Enums.Types;

namespace BusinessLogic.DTOs.User
{
    /// <summary>
    /// Represents a create DTO for user entities.
    /// </summary>
    public record UserCreateDto(
        string Name = "",
        string Email = "",
        string Password = "",
        UserRole Role = UserRole.Reader
        );
}
