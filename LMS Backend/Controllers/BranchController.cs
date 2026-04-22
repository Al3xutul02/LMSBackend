using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for branch-related endpoints
    /// </summary>
    /// <param name="branchService">The branch service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class BranchController(
        IBranchService branchService) : ControllerBase
    {
        private readonly IBranchService _branchService = branchService;

        /// <summary>
        /// Get a branch after its primary key
        /// </summary>
        /// <param name="id">Primary key of the branch</param>
        /// <returns>Action result with the response, branch read DTO if OK</returns>
        [HttpGet("get")]
        [ProducesResponseType(typeof(BranchReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                var branch = await _branchService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (branch == null) return NotFound($"No branch found with id {id}");

                return Ok(branch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all branches (WARNING: Because of the large volume of data, should only be used in testing.)
        /// </summary>
        /// <returns>Action result with the response, branch read DTO list if OK</returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<BranchReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var branches = await _branchService.GetAllAsync(IncludeBehavior.AllIncludes);

                return Ok(branches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a branch
        /// </summary>
        /// <param name="dto">Create DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(BranchCreateDto dto)
        {
            try
            {
                bool success = await _branchService.CreateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a branch
        /// </summary>
        /// <param name="dto">Update DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
        [HttpPut("put")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(BranchUpdateDto dto)
        {
            try
            {
                bool success = await _branchService.UpdateAsync(dto);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a branch
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

                bool success = await _branchService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
