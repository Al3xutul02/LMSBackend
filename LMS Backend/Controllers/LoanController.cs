using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for loan-related endpoints
    /// </summary>
    /// <param name="loanService">The loan service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class LoanController(
        ILoanService loanService) : ControllerBase
    {
        private readonly ILoanService _loanService = loanService;

        /// <summary>
        /// Get a loan after its primary key
        /// </summary>
        /// <param name="id">Primary key of the loan</param>
        /// <returns>Action result with the response, loan read DTO if OK</returns>
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
                if (loan == null) return NotFound($"No loan found with id {id}");

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all loans (WARNING: Because of the large volume of data, should only be used in testing.)
        /// </summary>
        /// <returns>Action result with the response, loan read DTO list if OK</returns>
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

        /// <summary>
        /// Create a loan
        /// </summary>
        /// <param name="dto">Create DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
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

        /// <summary>
        /// Update a loan
        /// </summary>
        /// <param name="dto">Update DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
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

        /// <summary>
        /// Delete a loan
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

                bool success = await _loanService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all active and overdue loans for a specific user
        /// </summary>
        /// <param name="userId">Primary key of the user</param>
        /// <returns>Action result with the response, list of loan read DTOs if OK</returns>
        [HttpGet("user-loans")]
        [ProducesResponseType(typeof(IEnumerable<LoanReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserLoans(int userId)
        {
            try
            {
                if (userId == 0) return BadRequest("Invalid user ID");

                var loans = await _loanService.GetUserLoansAsync(userId);

                return Ok(loans);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}