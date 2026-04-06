using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoanController(
        ILoanService loanService) : ControllerBase
    {
        private readonly ILoanService _loanService = loanService;

        [HttpGet("get")]
        [ProducesResponseType(typeof(LoanReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                var loan = await _loanService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (loan == null) return NotFound($"No user found with id {id}");

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<LoanReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var loans = await _loanService.GetAllAsync(IncludeBehavior.AllIncludes);

                return Ok(loans);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(LoanCreateDto dto)
        {
            try
            {
                bool success = await _loanService.CreateAsync(dto);

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
        public async Task<IActionResult> Put(LoanUpdateDto dto)
        {
            try
            {
                bool success = await _loanService.UpdateAsync(dto);

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

                bool success = await _loanService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
