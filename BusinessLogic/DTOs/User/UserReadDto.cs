using Repository.Enums.Types;

namespace BusinessLogic.DTOs.User
{
    public record UserReadDto(
        int Id = 0,
        string Name = "",
        string Email = "",
        UserRole Role = UserRole.Reader,
        int? EmployeeId = null,
        int? BranchId = null
        );
}
