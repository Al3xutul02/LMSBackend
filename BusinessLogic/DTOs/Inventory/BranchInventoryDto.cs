namespace BusinessLogic.DTOs.Inventory
{
    /// <summary>
    /// Represents inventory details for a specific branch.
    /// </summary>
    public record BranchInventoryDto(
        int BranchId = 0,
        string BranchName = "",
        int TotalBooks = 0,
        int UniqueBooks = 0,
        IEnumerable<BranchBookStockDto>? Books = null
    );
}
