using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories;
using Repository.Tables;

namespace LMSTests.Repository;

[TestClass]
public class UserRepositoryTests
{
    private static DatabaseContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DatabaseContext(options);
    }

    // ── BaseRepository methods ────────────────────────────────────────────────

    [TestMethod]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        using var ctx = CreateContext(nameof(GetByIdAsync_ExistingUser_ReturnsUser));
        ctx.Users.Add(new User { Name = "Alice", Email = "alice@test.com", PasswordHash = "hash", Role = UserRole.Reader });
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        var id = ctx.Users.First().Id;

        var result = await repo.GetByIdAsync(id, IncludeBehavior.NoInclude);

        Assert.IsNotNull(result);
        Assert.AreEqual("Alice", result.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        using var ctx = CreateContext(nameof(GetByIdAsync_NonExistingId_ReturnsNull));
        var repo = new UserRepository(ctx);

        var result = await repo.GetByIdAsync(999, IncludeBehavior.NoInclude);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAsync_MultipleUsers_ReturnsAll()
    {
        using var ctx = CreateContext(nameof(GetAllAsync_MultipleUsers_ReturnsAll));
        ctx.Users.AddRange(
            new User { Name = "Alice", Email = "a@test.com", PasswordHash = "h", Role = UserRole.Reader },
            new User { Name = "Bob", Email = "b@test.com", PasswordHash = "h", Role = UserRole.Librarian }
        );
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        var result = await repo.GetAllAsync(IncludeBehavior.NoInclude);

        Assert.AreEqual(2, result.Count());
    }

    [TestMethod]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmpty()
    {
        using var ctx = CreateContext(nameof(GetAllAsync_EmptyDatabase_ReturnsEmpty));
        var repo = new UserRepository(ctx);

        var result = await repo.GetAllAsync(IncludeBehavior.NoInclude);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task AddAsync_ValidUser_PersistsToDatabase()
    {
        using var ctx = CreateContext(nameof(AddAsync_ValidUser_PersistsToDatabase));
        var repo = new UserRepository(ctx);
        var user = new User { Name = "Charlie", Email = "c@test.com", PasswordHash = "h", Role = UserRole.Reader };

        await repo.AddAsync(user);
        await repo.SaveAsync();

        Assert.AreEqual(1, ctx.Users.Count());
        Assert.AreEqual("Charlie", ctx.Users.First().Name);
    }

    [TestMethod]
    public async Task Update_ExistingUser_UpdatesData()
    {
        using var ctx = CreateContext(nameof(Update_ExistingUser_UpdatesData));
        var user = new User { Name = "Dave", Email = "d@test.com", PasswordHash = "h", Role = UserRole.Reader };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        user.Name = "David";
        var repo = new UserRepository(ctx);
        repo.Update(user);
        await repo.SaveAsync();

        Assert.AreEqual("David", ctx.Users.First().Name);
    }

    [TestMethod]
    public async Task Delete_ExistingUser_RemovesFromDatabase()
    {
        using var ctx = CreateContext(nameof(Delete_ExistingUser_RemovesFromDatabase));
        var user = new User { Name = "Eve", Email = "e@test.com", PasswordHash = "h", Role = UserRole.Reader };
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        repo.Delete(user);
        await repo.SaveAsync();

        Assert.AreEqual(0, ctx.Users.Count());
    }

    // ── UserRepository-specific methods ──────────────────────────────────────

    [TestMethod]
    public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
    {
        using var ctx = CreateContext(nameof(EmailExistsAsync_ExistingEmail_ReturnsTrue));
        ctx.Users.Add(new User { Name = "Alice", Email = "alice@test.com", PasswordHash = "h", Role = UserRole.Reader });
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        var result = await repo.EmailExistsAsync("alice@test.com");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task EmailExistsAsync_NonExistingEmail_ReturnsFalse()
    {
        using var ctx = CreateContext(nameof(EmailExistsAsync_NonExistingEmail_ReturnsFalse));
        var repo = new UserRepository(ctx);

        var result = await repo.EmailExistsAsync("nobody@test.com");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetByUsernameAsync_ExistingName_ReturnsUser()
    {
        using var ctx = CreateContext(nameof(GetByUsernameAsync_ExistingName_ReturnsUser));
        ctx.Users.Add(new User { Name = "Frank", Email = "f@test.com", PasswordHash = "h", Role = UserRole.Reader });
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        var result = await repo.GetByUsernameAsync("Frank");

        Assert.IsNotNull(result);
        Assert.AreEqual("Frank", result.Name);
    }

    [TestMethod]
    public async Task GetByUsernameAsync_NonExistingName_ReturnsNull()
    {
        using var ctx = CreateContext(nameof(GetByUsernameAsync_NonExistingName_ReturnsNull));
        var repo = new UserRepository(ctx);

        var result = await repo.GetByUsernameAsync("Ghost");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetUserForProfileAsync_ExistingId_ReturnsUser()
    {
        using var ctx = CreateContext(nameof(GetUserForProfileAsync_ExistingId_ReturnsUser));
        ctx.Users.Add(new User { Name = "Grace", Email = "g@test.com", PasswordHash = "h", Role = UserRole.Reader });
        await ctx.SaveChangesAsync();

        var repo = new UserRepository(ctx);
        var id = ctx.Users.First().Id;
        var result = await repo.GetUserForProfileAsync(id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Grace", result.Name);
    }

    [TestMethod]
    public async Task GetUserForProfileAsync_NonExistingId_ReturnsNull()
    {
        using var ctx = CreateContext(nameof(GetUserForProfileAsync_NonExistingId_ReturnsNull));
        var repo = new UserRepository(ctx);

        var result = await repo.GetUserForProfileAsync(999);

        Assert.IsNull(result);
    }
}
