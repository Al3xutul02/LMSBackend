using Repository.Enums.Types;

namespace Repository.Tables
{
    public class BookGenre
    {
        public int BookISBN { get; set; }
        public Book? Book { get; set; }
        public BookGenreType Genre { get; set; }
    }
}
