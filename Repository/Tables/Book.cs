using Repository.Enums.Types;

namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>Books</c> table.
    /// </summary>
    public class Book
    {
        public int ISBN { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<BookGenre> Genres { get; set; } = [];
        public int Count { get; set; }
        public BookStatus Status { get; set; }

        /// <value>
        /// Represents relations with the intermediary table that helps
        /// connect with the <c>Branches</c> table in a many-to-many-relationship
        /// </value>
        public ICollection<BranchBookRelation> Branches { get; set; } = [];

        /// <value>
        /// Represents relations with the intermediary table that helps
        /// connect with the <c>Loans</c> table in a many-to-many-relationship
        /// </value>
        public ICollection<LoanBookRelation> Loans { get; set; } = [];
    }
}
