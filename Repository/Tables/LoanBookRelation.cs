namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>BranchBookRelations</c> intermediary table.
    /// </summary>
    public class LoanBookRelation
    {
        public int LoanId { get; set; }
        public Loan? Loan { get; set; }
        public int BookISBN { get; set; }
        public Book? Book { get; set; }
        public int Count { get; set; }
    }
}
