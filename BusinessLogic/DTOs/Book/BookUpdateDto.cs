using Repository.Enums.Types;

namespace BusinessLogic.DTOs.Book
{
    public record BookUpdateDto(
        int ISBN = 0,
        string Title = "",
        string Author = "",
        string Description = "",
        IEnumerable<BookGenreType>? Genres = null,
        int Count = 0,
        BookStatus Status = BookStatus.InStock
        );
}
