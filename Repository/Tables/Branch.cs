namespace Repository.Tables
{
    /// <summary>
    /// Represents data for entities in the <c>Branches</c> table.
    /// </summary>
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsOpen { get; set; }
        public ICollection<User> Librarians { get; set; } = [];

        /// <value>
        /// Represents relations with the intermediary table that helps
        /// connect with the <c>Books</c> table in a many-to-many-relationship
        /// </value>
        public ICollection<BranchBookRelation> Books { get; set; } = [];
    }
}
