using Repository.Enums.Types;

namespace Repository.Tables
{
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
        public ICollection<LoanBookRelation> Books { get; set; } = [];
    }
}
