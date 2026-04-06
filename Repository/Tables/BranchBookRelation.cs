namespace Repository.Tables
{
    public class BranchBookRelation
    {
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public int BookISBN { get; set; }
        public Book? Book { get; set; }
        public int Count { get; set; }
    }
}
