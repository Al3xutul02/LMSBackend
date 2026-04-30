using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    /// <summary>
    /// Represents a create DTO for branch entities.
    /// </summary>
    public record BranchCreateDto(
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
