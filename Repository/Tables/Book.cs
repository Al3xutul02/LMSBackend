using Repository.Enums.Types;

namespace Repository.Tables
{
    public class Book
    {
        public int ISBN { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<BookGenre> Genres { get; set; } = [];
        public int Count { get; set; }
        public BookStatus Status { get; set; }
        public ICollection<BranchBookRelation> Branches { get; set; } = [];
        public ICollection<LoanBookRelation> Loans { get; set; } = [];
    }
}
