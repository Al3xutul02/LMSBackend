using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Book
{
    /// <summary>
    /// Represents a create DTO for book entities.
    /// </summary>
    public record BookCreateDto(
        int ISBN = 0,
        string Title = "",
        string Author = "",
        string Description = "",
        IEnumerable<BookGenreType>? Genres = null,
        int Count = 0,
        BookStatus Status = BookStatus.InStock,
        string ImagePath = ""
        );
}
