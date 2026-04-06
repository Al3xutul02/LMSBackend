using BusinessLogic.DTOs.Book;

namespace BusinessLogic.DTOs.Branch
{
    public record BranchReadDto(
        int Id = 0,
        string Name = "",
        string Address = "",
        bool IsOpen = false,
        ICollection<int>? EmployeeIds = null,
        ICollection<BookRelationDto>? BookRelations = null
        );
}
