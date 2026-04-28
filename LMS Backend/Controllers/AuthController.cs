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
