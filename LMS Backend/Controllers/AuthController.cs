using BusinessLogic.DTOs.Login;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for authentication methods
    /// </summary>
    /// <param name="authService">The authentication service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Verifies the validity of a provided JWT bearer token.
        /// </summary>
        /// <returns>True if the token is valid; otherwise, a
        /// 400 Bad Request if the header is missing or the token is invalid.</returns>
        [HttpGet("is-logged-in")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IsLoggedIn()
        {
            string? authHeader = Request.Headers.Authorization;

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest("No Bearer token found.");
            }

            string token = authHeader["Bearer ".Length..].Trim();

            try
            {
                bool success = await _authService.IsLoggedIn(token);
                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user based on username and password.
        /// </summary>
        /// <param name="dto">The login credentials containing username and password.</param>
        /// <returns>A <see cref="LoginResponseDto"/> containing access and refresh tokens,
        /// or 404 if the user is not found.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest("Username or password invalid.");
                }

                var result = await _authService.Login(dto);
                if (result == null) return NotFound("Account with username and password was not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Registers a new user in the system and automatically logs them in.
        /// </summary>
        /// <param name="dto">The registration details including Name, Email, and Password.</param>
        /// <returns>A <see cref="LoginResponseDto"/> with tokens, or 400 if registration fails.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name)  ||
                    string.IsNullOrEmpty(dto.Email) ||
                    string.IsNullOrEmpty(dto.Password))
                {
                    return BadRequest("Invalid user credentials.");
                }

                var result = await _authService.Register(dto);
                if (result == null) return BadRequest("Could not create an account with that information.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Generates a new pair of access and refresh tokens using an existing refresh token and the expired access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token string provided by the client.</param>
        /// <returns>A new <see cref="LoginResponseDto"/> if valid; otherwise, a 400 or 404 error.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            string? authHeader = Request.Headers.Authorization;

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest("No Bearer token found.");
            }

            string token = authHeader["Bearer ".Length..].Trim();

            try
            {
                var result = await _authService.RefreshToken(new LoginResponseDto(token, refreshToken));
                if (result == null) return BadRequest("Invalid token or refresh token");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
