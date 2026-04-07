using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Book
{
    public record BookReadDto(
        int ISBN = 0,
        string Title = "",
        string Author = "",
        string Description = "",
        ICollection<BookGenreType>? Genres = null,
        int Count = 0,
        BookStatus Status = BookStatus.InStock,
        int? LoanDurationDays = null,
        bool? CanBeReserved = null
        );
}
