using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.DTOs.Login
{
    /// <summary>
    /// Represents the login response when logging into the application.
    /// </summary>
    public record LoginResponseDto(
        string Token = "",
        string RefreshToken = ""
        );
}
