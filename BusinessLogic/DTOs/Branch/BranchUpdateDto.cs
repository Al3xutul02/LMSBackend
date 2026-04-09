using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    public record BranchUpdateDto(
        int Id = 0,
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
