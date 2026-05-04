using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository.Enums.Types;
using Repository.Tables;
using System.Text.RegularExpressions;

namespace Repository.Contexts
{
    /// <summary>
    /// The Entity Framework Core database context for the Library Management System.
    /// </summary>
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
        public DbSet<Loan>               Loans               { get; set; }
        public DbSet<Fine>               Fines               { get; set; }
        public DbSet<Book>               Books               { get; set; }
        public DbSet<User>               Users               { get; set; }
        public DbSet<Branch>             Branches            { get; set; }
        public DbSet<LoanBookRelation>   LoanBookRelations   { get; set; }
        public DbSet<BranchBookRelation> BranchBookRelations { get; set; }
        public DbSet<BookGenre>          BookGenres          { get; set; }

        // Converts PascalCase enum names to kebab-case strings for DB storage
        // e.g. InStock → "in-stock", OutOfStock → "out-of-stock"
        private static string ToKebab(Enum value) =>
            Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();

        // Parses a kebab-case string back to an enum
        // e.g. "in-stock" → remove dashes → "instock" → InStock (case-insensitive)
        private static T FromKebab<T>(string value) where T : struct =>
            Enum.Parse<T>(value.Replace("-", ""), ignoreCase: true);

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
                entity.Property(e => e.RefreshToken)
                    .HasDefaultValue(null);
                entity.Property(e => e.RefreshTokenExpiryTime)
                    .HasDefaultValue(null);
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasConversion(
                        v => ToKebabCase(v.ToString()),
                        v => EnumParse<UserRole>(v)
                    );
                entity.Property(e => e.ImagePath)
                    .HasDefaultValue(null);
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
                entity.Property(e => e.ImagePath)
                    .HasDefaultValue(null);
            });

            // ── Enum value converters (DB stores kebab-case strings) ───────

            var bookStatusConverter = new ValueConverter<BookStatus, string>(
                v => ToKebab(v),
                v => FromKebab<BookStatus>(v));

            var loanStatusConverter = new ValueConverter<LoanStatus, string>(
                v => ToKebab(v),
                v => FromKebab<LoanStatus>(v));

            var fineStatusConverter = new ValueConverter<FineStatus, string>(
                v => ToKebab(v),
                v => FromKebab<FineStatus>(v));

            var userRoleConverter = new ValueConverter<UserRole, string>(
                v => ToKebab(v),
                v => FromKebab<UserRole>(v));

            modelBuilder.Entity<Book>()
                .Property(b => b.Status)
                .HasConversion(bookStatusConverter);

            modelBuilder.Entity<Loan>()
                .Property(l => l.Status)
                .HasConversion(loanStatusConverter);

            modelBuilder.Entity<Fine>()
                .Property(f => f.Status)
                .HasConversion(fineStatusConverter);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(userRoleConverter);

            // BookGenreType is stored as integer in the DB — EF default mapping handles this.

            // ── Fine → Loan one-to-one relationship ───────────────────────
            // Fine is the dependent side (owns LoanId FK).
            // Loan.FineId is a plain column, not a second FK.
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Loan)
                .WithOne(l => l.Fine)
                .HasForeignKey<Fine>(f => f.LoanId)
                .IsRequired(false);

            // ── Composite primary keys for junction tables ─────────────────
            modelBuilder.Entity<LoanBookRelation>()
                .HasKey(r => new { r.LoanId, r.BookISBN });

            modelBuilder.Entity<BranchBookRelation>()
                .HasKey(r => new { r.BranchId, r.BookISBN });

            modelBuilder.Entity<BookGenre>()
                .HasKey(g => new { g.BookISBN, g.Genre });
        }
    }
}
