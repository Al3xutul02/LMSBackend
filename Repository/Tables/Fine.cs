using Repository.Enums.Types;

namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>Fines</c> table.
    /// </summary>
    public class Fine
    {
        public int Id { get; set; }
        public int? LoanId { get; set; }
        public Loan? Loan { get; set; }
        public int Amount { get; set; }
        public FineStatus Status { get; set; }
    }
}
