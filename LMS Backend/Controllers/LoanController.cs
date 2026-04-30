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

        #region Standard CRUD Endpoints

        /// <summary>
        /// Get a loan after its primary key
        /// </summary>
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

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<LoanReadDto>), StatusCodes.Status200OK)]
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

        #endregion

        #region Librarian Workflow Endpoints

        /// <summary>
        /// Gets detailed information for a specific loan request (used in Request Details page).
        /// </summary>
        /// <param name="id">The ID of the loan request</param>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(LoanDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequestDetails(int id)
        {
            try
            {
                var details = await _loanService.GetRequestDetailsAsync(id);
                if (details == null) return NotFound($"Request with ID {id} not found.");

                return Ok(details);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Approves a loan request, updates stock and sets the return deadline.
        /// </summary>
        [HttpPatch("{id}/approve")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                bool success = await _loanService.ApproveRequestAsync(id);
                if (!success) return BadRequest("Could not approve request. Check stock or request status.");

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Rejects a loan request.
        /// </summary>
        [HttpPatch("{id}/reject")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                bool success = await _loanService.RejectRequestAsync(id);
                if (!success) return BadRequest("Could not reject request.");

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}