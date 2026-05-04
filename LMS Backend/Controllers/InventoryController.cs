using BusinessLogic.DTOs.Inventory;
using BusinessLogic.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace LMS_Backend.Controllers
{
    /// <summary>
    /// API Controller for inventory management endpoints
    /// </summary>
    /// <param name="inventoryService">The inventory service used by the controller</param>
    [ApiController]
    [Route("[controller]")]
    public class InventoryController(
        IInventoryService inventoryService) : ControllerBase
    {
        private readonly IInventoryService _inventoryService = inventoryService;

        /// <summary>
        /// Get inventory statistics across all branches
        /// </summary>
        /// <returns>Action result with inventory stats DTO if OK</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(InventoryStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _inventoryService.GetInventoryStatsAsync();
                if (stats == null) return BadRequest("Failed to retrieve inventory statistics");

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get inventory for a specific branch
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <returns>Action result with branch inventory DTO if OK</returns>
        [HttpGet("branch/{branchId}")]
        [ProducesResponseType(typeof(BranchInventoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBranchInventory(int branchId)
        {
            try
            {
                if (branchId == 0) return BadRequest("Invalid branch ID");

                var inventory = await _inventoryService.GetBranchInventoryAsync(branchId);
                if (inventory == null) return NotFound($"No branch found with ID {branchId}");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get stock details for a specific book at a specific branch
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <returns>Action result with book stock DTO if OK</returns>
        [HttpGet("branch/{branchId}/book/{bookISBN}")]
        [ProducesResponseType(typeof(BranchBookStockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookStock(int branchId, int bookISBN)
        {
            try
            {
                if (branchId == 0 || bookISBN == 0) return BadRequest("Invalid branch ID or book ISBN");

                var stock = await _inventoryService.GetBookStockAsync(branchId, bookISBN);
                if (stock == null) return NotFound($"No book found with ISBN {bookISBN} at branch {branchId}");

                return Ok(stock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update inventory (add or remove books from a branch)
        /// </summary>
        /// <param name="dto">Update inventory DTO</param>
        /// <returns>Action result with confirmation of the action if OK</returns>
        [HttpPost("update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInventory(UpdateInventoryDto dto)
        {
            try
            {
                if (dto.BranchId == 0 || dto.BookISBN == 0) return BadRequest("Invalid branch ID or book ISBN");

                bool success = await _inventoryService.UpdateInventoryAsync(dto);

                if (!success) return BadRequest("Failed to update inventory");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Add books to a branch inventory
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <param name="count">Number of books to add</param>
        /// <returns>Action result with confirmation if OK</returns>
        [HttpPost("add/{branchId}/{bookISBN}/{count}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBooks(int branchId, int bookISBN, int count)
        {
            try
            {
                if (branchId == 0 || bookISBN == 0 || count <= 0)
                    return BadRequest("Invalid parameters");

                bool success = await _inventoryService.AddBooksAsync(branchId, bookISBN, count);

                if (!success) return BadRequest("Failed to add books");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Remove books from a branch inventory
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <param name="count">Number of books to remove</param>
        /// <returns>Action result with confirmation if OK</returns>
        [HttpPost("remove/{branchId}/{bookISBN}/{count}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveBooks(int branchId, int bookISBN, int count)
        {
            try
            {
                if (branchId == 0 || bookISBN == 0 || count <= 0)
                    return BadRequest("Invalid parameters");

                bool success = await _inventoryService.RemoveBooksAsync(branchId, bookISBN, count);

                if (!success) return BadRequest("Failed to remove books");

                return Ok(success);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all inventory items across all branches
        /// </summary>
        /// <returns>Action result with list of book stock DTOs if OK</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<BranchBookStockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllInventory()
        {
            try
            {
                var inventory = await _inventoryService.GetAllBranchBookInventoriesAsync();

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get low stock items (books below threshold)
        /// </summary>
        /// <param name="threshold">Minimum stock threshold (default 5)</param>
        /// <returns>Action result with list of low stock items if OK</returns>
        [HttpGet("low-stock")]
        [ProducesResponseType(typeof(IEnumerable<BranchBookStockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLowStockItems([FromQuery] int threshold = 5)
        {
            try
            {
                if (threshold < 0) threshold = 5;

                var lowStockItems = await _inventoryService.GetLowStockItemsAsync(threshold);

                return Ok(lowStockItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}