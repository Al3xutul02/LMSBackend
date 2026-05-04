namespace BusinessLogic.DTOs.Login
{
    /// <summary>
    /// Represents a login DTO for user information.
    /// </summary>
    public record LoginDto(
        string Username = "",
        string Password = ""
        );
}
