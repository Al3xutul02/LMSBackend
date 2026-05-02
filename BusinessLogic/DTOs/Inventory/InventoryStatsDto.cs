namespace BusinessLogic.DTOs.Inventory
{
    /// <summary>
    /// Represents inventory statistics for the library.
    /// </summary>
    public record InventoryStatsDto(
        int TotalBooks = 0,
        int AvailableBooks = 0,
        int BorrowedBooks = 0,
        int TotalBranches = 0,
        IEnumerable<BranchInventoryDto>? BranchInventories = null
    );
}
