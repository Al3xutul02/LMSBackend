using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    public record BranchCreateDto(
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        ICollection<BookRelationDto>? BookRelations = null
        );
}
