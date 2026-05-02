using BusinessLogic.DTOs.Inventory;

namespace BusinessLogic.Services.Abstract
{
    /// <summary>
    /// Defines the interface for inventory management services.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets inventory statistics across all branches.
        /// </summary>
        /// <returns>Inventory statistics DTO</returns>
        Task<InventoryStatsDto?> GetInventoryStatsAsync();

        /// <summary>
        /// Gets inventory details for a specific branch.
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <returns>Branch inventory DTO</returns>
        Task<BranchInventoryDto?> GetBranchInventoryAsync(int branchId);

        /// <summary>
        /// Gets stock details for a specific book at a specific branch.
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <returns>Branch book stock DTO</returns>
        Task<BranchBookStockDto?> GetBookStockAsync(int branchId, int bookISBN);

        /// <summary>
        /// Updates inventory (add or remove books from branch).
        /// </summary>
        /// <param name="dto">Update inventory DTO</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> UpdateInventoryAsync(UpdateInventoryDto dto);

        /// <summary>
        /// Adds books to a branch inventory.
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <param name="count">Number of books to add</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> AddBooksAsync(int branchId, int bookISBN, int count);

        /// <summary>
        /// Removes books from a branch inventory.
        /// </summary>
        /// <param name="branchId">The branch ID</param>
        /// <param name="bookISBN">The book ISBN</param>
        /// <param name="count">Number of books to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> RemoveBooksAsync(int branchId, int bookISBN, int count);

        /// <summary>
        /// Gets all books across all branches (inventory view).
        /// </summary>
        /// <returns>Enumerable of book stock DTOs</returns>
        Task<IEnumerable<BranchBookStockDto>> GetAllBranchBookInventoriesAsync();

        /// <summary>
        /// Gets low stock items (books with count below threshold).
        /// </summary>
        /// <param name="threshold">Minimum stock threshold</param>
        /// <returns>Enumerable of low stock items</returns>
        Task<IEnumerable<BranchBookStockDto>> GetLowStockItemsAsync(int threshold = 5);
    }
}
