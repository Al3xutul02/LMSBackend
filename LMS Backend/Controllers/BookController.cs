using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for book-related endpoints
    /// </summary>
    /// <param name="bookService">The book service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class BookController(
        IBookService bookService) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;

        /// <summary>
        /// Get a book after its primary key
        /// </summary>
        /// <param name="id">Primary key of the book</param>
        /// <returns>Action result with the response, book read DTO if OK</returns>
        [HttpGet("get")]
        [ProducesResponseType(typeof(BookReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0) return BadRequest("Invalid ISBN");

                var book = await _bookService.GetByIdAsync(id, IncludeBehavior.AllIncludes);
                if (book == null) return NotFound($"No book found with ISBN {id}");

                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all books (WARNING: Because of the large volume of data, should only be used in testing.)
        /// </summary>
        /// <returns>Action result with the response, book read DTO list if OK</returns>
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

        /// <summary>
        /// Create a book
        /// </summary>
        /// <param name="dto">Create DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
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

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param name="dto">Update DTO needed</param>
        /// <returns>Action result with the response, confirmation of the action if OK</returns>
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
        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var books = await _bookService.GetAllAsync(IncludeBehavior.NoInclude);
                var totalBooks = books.Count();
                var booksAvailable = books.Count(b => b.Status == BookStatus.InStock);

                return Ok(new { totalBooks, booksAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Delete a book
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
                if (id == 0) return BadRequest("Invalid ISBN");

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
