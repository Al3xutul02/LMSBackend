using Repository.Enums.Types;

namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>BookGenres</c> table.
    /// </summary>
    public class BookGenre
    {
        public int BookISBN { get; set; }
        public Book? Book { get; set; }
        public BookGenreType Genre { get; set; }
    }
}
