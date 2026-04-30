using BusinessLogic.DTOs.BorrowRequest;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMS_Backend.Controllers
{
    [ApiController]
    [Route("borrow-request")]
    public class BorrowRequestController(IBorrowRequestService borrowRequestService) : ControllerBase
    {
        private readonly IBorrowRequestService _borrowRequestService = borrowRequestService;

        [HttpGet("get")]
        [ProducesResponseType(typeof(BorrowRequestReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                var request = await _borrowRequestService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (request == null) return NotFound($"No request found with id {id}");
                return Ok(request);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("get-pending")]
        [ProducesResponseType(typeof(IEnumerable<BorrowRequestReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var requests = await _borrowRequestService.GetByStatusAsync(RequestStatus.Pending);
                return Ok(requests);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<BorrowRequestReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var requests = await _borrowRequestService.GetAllAsync(IncludeBehavior.AllIncludes);
                return Ok(requests);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post(BorrowRequestCreateDto dto)
        {
            try
            {
                bool success = await _borrowRequestService.CreateAsync(dto);
                return Ok(success);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("finish")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Finish(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                bool success = await _borrowRequestService.FinishAsync(id);
                if (!success) return BadRequest("Could not finish request. It may already be processed or the book is out of stock.");
                return Ok(success);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("reject")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                bool success = await _borrowRequestService.RejectAsync(id);
                if (!success) return BadRequest("Could not reject request. It may already be processed.");
                return Ok(success);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("delete")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");
                bool success = await _borrowRequestService.DeleteAsync(id);
                return Ok(success);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}