namespace Repository.Tables
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsOpen { get; set; }
        public ICollection<User> Librarians { get; set; } = [];
        public ICollection<BranchBookRelation> Books { get; set; } = [];
    }
}
