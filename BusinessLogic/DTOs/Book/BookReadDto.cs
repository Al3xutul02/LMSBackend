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
        BookStatus Status = BookStatus.InStock,
        int? LoanDurationDays = null,
        bool? CanBeReserved = null
        );
}
