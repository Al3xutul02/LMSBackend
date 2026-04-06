namespace BusinessLogic.DTOs.User
{
    public record UserUpdateDto(
        int Id = 0,
        string Name = "",
        string Email = "",
        string Password = ""
        );
}
