namespace BusinessLogic.DTOs.Inventory
{
    /// <summary>
    /// Represents stock details for a specific book at a specific branch.
    /// </summary>
    public record BranchBookStockDto(
        int BookISBN = 0,
        string BookTitle = "",
        string BookAuthor = "",
        int AvailableCount = 0,
        int TotalCount = 0,
        int BorrowedCount = 0
    );
}
