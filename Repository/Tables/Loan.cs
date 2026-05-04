using Repository.Enums.Types;

namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>Loans</c> table.
    /// </summary>
    public class Loan
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? FineId { get; set; }
        public Fine? Fine { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public LoanStatus Status { get; set; }

        /// <value>
        /// Represents relations with the intermediary table that helps
        /// connect with the <c>Books</c> table in a many-to-many-relationship
        /// </value>
        public ICollection<LoanBookRelation> Books { get; set; } = [];
    }
}