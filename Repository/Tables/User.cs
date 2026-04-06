using Repository.Enums.Types;

namespace Repository.Tables
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public int? EmployeeId { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public ICollection<Loan> Loans { get; set; } = [];
    }
}
