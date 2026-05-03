using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Book
{
    /// <summary>
    /// Represents a read DTO for book entities.
    /// </summary>
    public record BookReadDto(
        int ISBN = 0,
        string Title = "",
        string Author = "",
        string Description = "",
        IEnumerable<BookGenreType>? Genres = null,
        int Count = 0,
        IEnumerable<string>? Branches = null,
        BookStatus Status = BookStatus.InStock,
        string ImagePath = "",
        int? LoanDurationDays = null,
        bool? CanBeReserved = null
        );
}
