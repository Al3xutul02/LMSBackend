using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BranchController(
        IBranchService branchService) : ControllerBase
    {
        private readonly IBranchService _branchService = branchService;

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
                if (branch == null) return NotFound($"No user found with id {id}");

                return Ok(branch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
