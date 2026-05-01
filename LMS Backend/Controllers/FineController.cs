using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for fine-related endpoints
    /// </summary>
    /// <param name="fineService">The fine service used by the controller</param>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FineController(
        IFineService fineService) : ControllerBase
    {
        private readonly IFineService _fineService = fineService;

        /// <summary>
        /// Get a fine after its primary key
        /// </summary>
        /// <param name="id">Primary key of the fine</param>
        /// <returns>Action result with the response, fine read DTO if OK</returns>
        [HttpGet("get")]
        [ProducesResponseType(typeof(FineReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                var fine = await _fineService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (fine == null) return NotFound($"No fine found with id {id}");

                return Ok(fine);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all fines (WARNING: Because of the large volume of data, should only be used in testing.)
        /// </summary>
        /// <returns>Action result with the response, fine read DTO list if OK</returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<FineReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var fines = await _fineService.GetAllAsync(IncludeBehavior.AllIncludes);

                return Ok(fines);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a fine
        /// </summary>
        /// <param name="dto">Create DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [Authorize(Roles = "Librarian")]
        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(FineCreateDto dto)
        {
            try
            {
                bool success = await _fineService.CreateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a fine
        /// </summary>
        /// <param name="dto">Update DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [Authorize(Roles = "Librarian")]
        [HttpPut("put")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(FineUpdateDto dto)
        {
            try
            {
                bool success = await _fineService.UpdateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a fine
        /// </summary>
        /// <param name="id">Primary key needed for deletion</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [Authorize(Roles = "Librarian")]
        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                bool success = await _fineService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
