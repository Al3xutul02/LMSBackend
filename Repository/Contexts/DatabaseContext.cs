using Microsoft.EntityFrameworkCore;
using Repository.Enums.Types;
using Repository.Tables;
using System.Text.RegularExpressions;

namespace Repository.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Loan> Loans => Set<Loan>();
        public DbSet<Fine> Fines => Set<Fine>();
        public DbSet<BookGenre> BookGenres => Set<BookGenre>();
        public DbSet<BranchBookRelation> BranchBookRelations => Set<BranchBookRelation>();
        public DbSet<LoanBookRelation> loanBookRelations => Set<LoanBookRelation>();

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity => {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnType("text");
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<UserRole>(v)
                    );
                entity.Property(e => e.EmployeeId);
                entity.HasOne(d => d.Branch).WithMany(p => p.Librarians)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Book>(entity => {
                entity.ToTable("Books");
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text");
                entity.Property(e => e.Count)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<BookStatus>(v)
                    );
            });

            modelBuilder.Entity<Branch>(entity => {
                entity.ToTable("Branches");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.IsOpen)
                    .IsRequired()
                    .HasColumnType("boolean");
            });

            modelBuilder.Entity<Loan>(entity => {
                entity.ToTable("Loans");
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.User).WithMany(p => p.Loans)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(d => d.Fine).WithOne(p => p.Loan)
                    .HasForeignKey<Loan>(d => d.FineId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.IssueDate)
                    .IsRequired();
                entity.Property(e => e.DueDate)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<LoanStatus>(v)
                    );
            });

            modelBuilder.Entity<Fine>(entity => {
                entity.ToTable("Fines");
                entity.HasKey(e => e.Id);
                entity.HasOne(d => d.Loan).WithOne(p => p.Fine)
                    .HasForeignKey<Fine>(d => d.LoanId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.Amount)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<FineStatus>(v)
                    );
            });

            modelBuilder.Entity<BookGenre>(entity => {
                entity.ToTable("BookGenres");
                entity.HasKey(e => new { e.BookISBN, e.Genre });
                entity.HasOne(d => d.Book).WithMany(p => p.Genres)
                    .HasForeignKey(d => d.BookISBN)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Genre)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<BookGenreType>(v)
                    );
            });

            modelBuilder.Entity<BranchBookRelation>(entity => {
                entity.ToTable("BranchBookRelations");
                entity.HasKey(e => new { e.BranchId, e.BookISBN });
                entity.Property(e => e.Count)
                    .IsRequired()
                    .HasDefaultValue(0);
                entity.HasOne(d => d.Branch).WithMany(p => p.Books)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Book).WithMany(p => p.Branches)
                    .HasForeignKey(d => d.BookISBN)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LoanBookRelation>(entity => {
                entity.ToTable("LoanBookRelations");
                entity.HasKey(e => new { e.LoanId, e.BookISBN });
                entity.Property(e => e.Count)
                    .IsRequired()
                    .HasDefaultValue(1);
                entity.HasOne(d => d.Loan).WithMany(p => p.Books)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                    .HasForeignKey(d => d.BookISBN)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        // Helper to convert PascalCase to kebab-case
        private static string ToKebabCase(string value) =>
            Regex.Replace(value, "(?<!^)([A-Z])", "-$1").ToLower();

        // Helper to convert kebab-case back to Enum
        private static T EnumParse<T>(string value) where T : struct, Enum =>
            Enum.TryParse<T>(value.Replace("-", ""), true, out var result) ? result : default;
    }
}
