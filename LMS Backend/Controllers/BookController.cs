using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMS_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController(
        IBookService bookService) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;

        [HttpGet("get")]
        [ProducesResponseType(typeof(BookReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ID");

                var book = await _bookService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (book == null) return NotFound($"No user found with id {id}");

                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<BookReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var books = await _bookService.GetAllAsync(IncludeBehavior.AllIncludes);

                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("post")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(BookCreateDto dto)
        {
            try
            {
                bool success = await _bookService.CreateAsync(dto);

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
        public async Task<IActionResult> Put(BookUpdateDto dto)
        {
            try
            {
                bool success = await _bookService.UpdateAsync(dto);

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

                bool success = await _bookService.DeleteAsync(id);

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
