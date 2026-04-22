using BusinessLogic.DTOs.User;
using BusinessLogic.Services;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for user-related endpoints
    /// </summary>
    /// <param name="userService">The user service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class UserController(
        IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Get a user after its primary key
        /// </summary>
        /// <param name="id">Primary key of the user</param>
        /// <returns>Action result with the response, user read DTO if OK</returns>
        [HttpGet("get")]
        [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                var user = await _userService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (user == null) return NotFound($"No user found with id {id}");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all users (WARNING: Because of the large volume of data, should only be used in testing.)
        /// </summary>
        /// <returns>Action result with the response, user read DTO list if OK</returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<UserReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync(IncludeBehavior.AllIncludes);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="dto">Create DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(UserCreateDto dto)
        {
            try
            {
                bool success = await _userService.CreateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="dto">Update DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [HttpPut("put")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(UserUpdateDto dto)
        {
            try
            {
                bool success = await _userService.UpdateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">Primary key needed for deletion</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                bool success = await _userService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-profile/{id}")]
        public async Task<ActionResult<UserReadDto>> GetMyProfile(int id)
        {
            var profile = await _userService.GetUserProfileAsync(id);

            if (profile == null)
            {
                return NotFound("Utilizatorul nu a fost găsit.");
            }

            return Ok(profile);
        }
    }
}
