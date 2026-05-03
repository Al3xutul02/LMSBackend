using Repository.Enums.Types;

namespace BusinessLogic.DTOs.User
{
    /// <summary>
    /// Represents a read DTO for user entities.
    /// </summary>
    public record UserReadDto(
        int Id = 0,
        string Name = "",
        string Email = "",
        UserRole Role = UserRole.Reader,
        string ImagePath = "",
        int? EmployeeId = null,
        int? BranchId = null
        );
}
