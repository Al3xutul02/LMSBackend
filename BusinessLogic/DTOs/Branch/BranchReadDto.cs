using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    public record BranchReadDto(
        int Id = 0,
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        IEnumerable<int>? EmployeeIds = null,
        IEnumerable<BookRelationDto>? BookRelations = null
        );
}
