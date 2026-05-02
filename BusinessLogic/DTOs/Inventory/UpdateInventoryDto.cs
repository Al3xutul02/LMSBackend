namespace BusinessLogic.DTOs.Inventory
{
    /// <summary>
    /// Represents a request to update inventory at a branch.
    /// </summary>
    public record UpdateInventoryDto(
        int BranchId = 0,
        int BookISBN = 0,
        int Count = 0,
        string? Reason = null
    );
}