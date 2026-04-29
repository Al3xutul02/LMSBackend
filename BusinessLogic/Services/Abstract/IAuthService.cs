using BusinessLogic.DTOs.Login;
using BusinessLogic.DTOs.User;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Authentication service interface, implemented by <see cref="AuthService"/>
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Validates a raw JWT string against the server's security key and validation parameters.
        /// </summary>
        /// <param name="token">The JWT string to verify.</param>
        /// <returns>True if the token is valid and not expired; otherwise, false.</returns>
        Task<bool> IsLoggedIn(string token);

        /// <summary>
        /// Verifies user credentials and issues a new pair of Access and Refresh tokens.
        /// </summary>
        /// <param name="dto">The login credentials.</param>
        /// <returns>A <see cref="LoginResponseDto"/> containing tokens, or null if unauthorized.</returns>
        Task<LoginResponseDto?> Login(LoginDto dto);

        /// <summary>
        /// Creates a new user record and returns an initial set of authentication tokens.
        /// </summary>
        /// <param name="dto">The user details for registration.</param>
        /// <returns>A <see cref="LoginResponseDto"/> if successful; null if the user already exists.</returns>
        Task<LoginResponseDto?> Register(UserCreateDto dto);

        /// <summary>
        /// Validates an expired access token and a refresh token to provide a new token pair.
        /// This implements "Refresh Token Rotation" for enhanced security.
        /// </summary>
        /// <param name="oldResponse">The expired Access Token and the current Refresh Token.</param>
        /// <returns>A new <see cref="LoginResponseDto"/>, or null if the refresh token is invalid/expired.</returns>
        Task<LoginResponseDto?> RefreshToken(LoginResponseDto oldResponse);
    }
}
