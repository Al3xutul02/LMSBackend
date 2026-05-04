namespace BusinessLogic.DTOs.User
{
    /// <summary>
    /// Represents an update DTO for user entities.
    /// </summary>
    public record UserUpdateDto(
        int Id = 0,
        string Name = "",
        string Email = "",
        string? Password = null,
        string ImagePath = "",
        int? EmployeeId = null,
        int? BranchId = null
        );
}
