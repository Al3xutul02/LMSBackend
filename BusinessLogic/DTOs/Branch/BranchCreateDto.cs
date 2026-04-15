using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    public record BranchCreateDto(
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
