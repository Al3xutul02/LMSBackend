using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    /// <summary>
    /// Represents an update DTO for branch entities.
    /// </summary>
    public record BranchUpdateDto(
        int Id = 0,
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
