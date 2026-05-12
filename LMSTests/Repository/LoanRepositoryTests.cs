using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories;
using Repository.Tables;

namespace LMSTests.Repository;

[TestClass]
public class LoanRepositoryTests
{
    private static DatabaseContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DatabaseContext(options);
    }

    private static Loan MakeLoan(LoanStatus status, int? userId = null)
        => new()
        {
            UserId = userId,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = status
        };

    [TestMethod]
    public async Task GetByStatusAsync_ReturnsLoansWithMatchingStatus()
    {
        using var ctx = CreateContext(nameof(GetByStatusAsync_ReturnsLoansWithMatchingStatus));
        ctx.Loans.AddRange(MakeLoan(LoanStatus.Active), MakeLoan(LoanStatus.Returned), MakeLoan(LoanStatus.Active));
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);
        var result = await repo.GetByStatusAsync(LoanStatus.Active);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(l => l.Status == LoanStatus.Active));
    }

    [TestMethod]
    public async Task GetByStatusAsync_NoMatchingStatus_ReturnsEmpty()
    {
        using var ctx = CreateContext(nameof(GetByStatusAsync_NoMatchingStatus_ReturnsEmpty));
        ctx.Loans.Add(MakeLoan(LoanStatus.Returned));
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);
        var result = await repo.GetByStatusAsync(LoanStatus.Active);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task HasUnpaidFinesAsync_UserHasUnpaidFine_ReturnsTrue()
    {
        using var ctx = CreateContext(nameof(HasUnpaidFinesAsync_UserHasUnpaidFine_ReturnsTrue));
        var user = new User { Name = "Alice", Email = "a@test.com", PasswordHash = "h", Role = UserRole.Reader };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var fine = new Fine { Amount = 10, Status = FineStatus.Unpaid };
        ctx.Fines.Add(fine);
        await ctx.SaveChangesAsync();

        var loan = new Loan
        {
            UserId = user.Id,
            FineId = fine.Id,
            Fine = fine, // set navigation property so InMemory can resolve it
            IssueDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(-16),
            Status = LoanStatus.Overdue
        };
        ctx.Loans.Add(loan);
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);
        var result = await repo.HasUnpaidFinesAsync(user.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task HasUnpaidFinesAsync_UserHasPaidFine_ReturnsFalse()
    {
        using var ctx = CreateContext(nameof(HasUnpaidFinesAsync_UserHasPaidFine_ReturnsFalse));
        var user = new User { Name = "Bob", Email = "b@test.com", PasswordHash = "h", Role = UserRole.Reader };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var fine = new Fine { Amount = 10, Status = FineStatus.Paid };
        ctx.Fines.Add(fine);
        await ctx.SaveChangesAsync();

        var loan = new Loan
        {
            UserId = user.Id,
            FineId = fine.Id,
            IssueDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(-16),
            Status = LoanStatus.Returned
        };
        ctx.Loans.Add(loan);
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);
        var result = await repo.HasUnpaidFinesAsync(user.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task HasUnpaidFinesAsync_UserHasNoLoans_ReturnsFalse()
    {
        using var ctx = CreateContext(nameof(HasUnpaidFinesAsync_UserHasNoLoans_ReturnsFalse));
        var repo = new LoanRepository(ctx);

        var result = await repo.HasUnpaidFinesAsync(42);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetLoansByUserIdAsync_ReturnsLoansForUser()
    {
        using var ctx = CreateContext(nameof(GetLoansByUserIdAsync_ReturnsLoansForUser));
        var user = new User { Name = "Carol", Email = "c@test.com", PasswordHash = "h", Role = UserRole.Reader };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        ctx.Loans.AddRange(
            new Loan { UserId = user.Id, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), Status = LoanStatus.Active },
            new Loan { UserId = user.Id, IssueDate = DateTime.UtcNow.AddDays(-30), DueDate = DateTime.UtcNow.AddDays(-16), Status = LoanStatus.Returned },
            new Loan { UserId = null, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), Status = LoanStatus.Pending }
        );
        await ctx.SaveChangesAsync();

        var repo = new LoanRepository(ctx);
        var result = await repo.GetLoansByUserIdAsync(user.Id);

        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.All(l => l.UserId == user.Id));
    }

    [TestMethod]
    public async Task GetLoansByUserIdAsync_UserWithNoLoans_ReturnsEmpty()
    {
        using var ctx = CreateContext(nameof(GetLoansByUserIdAsync_UserWithNoLoans_ReturnsEmpty));
        var repo = new LoanRepository(ctx);

        var result = await repo.GetLoansByUserIdAsync(999);

        Assert.AreEqual(0, result.Count());
    }
}
