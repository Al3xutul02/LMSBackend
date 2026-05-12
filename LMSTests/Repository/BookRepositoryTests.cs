using Microsoft.EntityFrameworkCore;
using Repository.Contexts;
using Repository.Enums.Types;
using Repository.Repositories;
using Repository.Tables;

namespace LMSTests.Repository;

[TestClass]
public class BookRepositoryTests
{
    private static DatabaseContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DatabaseContext(options);
    }

    private static Book MakeBook(int isbn, string title, string author, BookStatus status = BookStatus.InStock)
        => new() { ISBN = isbn, Title = title, Author = author, Description = "Desc", Count = 5, Status = status };

    [TestMethod]
    public async Task GetAllWithFiltersAsync_ByTitle_ReturnsMatchingBooks()
    {
        using var ctx = CreateContext(nameof(GetAllWithFiltersAsync_ByTitle_ReturnsMatchingBooks));
        ctx.Books.AddRange(MakeBook(1, "Clean Code", "Martin"), MakeBook(2, "Pragmatic Programmer", "Hunt"));
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetAllWithFiltersAsync("Clean", null, null);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Clean Code", result.First().Title);
    }

    [TestMethod]
    public async Task GetAllWithFiltersAsync_ByAuthor_ReturnsMatchingBooks()
    {
        using var ctx = CreateContext(nameof(GetAllWithFiltersAsync_ByAuthor_ReturnsMatchingBooks));
        ctx.Books.AddRange(MakeBook(1, "Book A", "Martin"), MakeBook(2, "Book B", "Hunt"));
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetAllWithFiltersAsync(null, "Hunt", null);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Hunt", result.First().Author);
    }

    [TestMethod]
    public async Task GetAllWithFiltersAsync_NoFilters_ReturnsAllBooks()
    {
        using var ctx = CreateContext(nameof(GetAllWithFiltersAsync_NoFilters_ReturnsAllBooks));
        ctx.Books.AddRange(MakeBook(1, "Book A", "Auth A"), MakeBook(2, "Book B", "Auth B"), MakeBook(3, "Book C", "Auth C"));
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetAllWithFiltersAsync(null, null, null);

        Assert.AreEqual(3, result.Count());
    }

    [TestMethod]
    public async Task GetAllWithFiltersAsync_NoMatch_ReturnsEmpty()
    {
        using var ctx = CreateContext(nameof(GetAllWithFiltersAsync_NoMatch_ReturnsEmpty));
        ctx.Books.Add(MakeBook(1, "Clean Code", "Martin"));
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetAllWithFiltersAsync("Dune", null, null);

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetByGenresAsync_MatchingGenre_ReturnsBooks()
    {
        using var ctx = CreateContext(nameof(GetByGenresAsync_MatchingGenre_ReturnsBooks));
        var book = MakeBook(1, "Sci-Fi Book", "Author");
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        ctx.BookGenres.Add(new BookGenre { BookISBN = 1, Genre = BookGenreType.ScienceFiction });
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetByGenresAsync(new List<int> { (int)BookGenreType.ScienceFiction });

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public async Task GetByGenresAsync_NoMatchingGenre_ReturnsEmpty()
    {
        using var ctx = CreateContext(nameof(GetByGenresAsync_NoMatchingGenre_ReturnsEmpty));
        var book = MakeBook(1, "Thriller Book", "Author");
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        ctx.BookGenres.Add(new BookGenre { BookISBN = 1, Genre = BookGenreType.Thriller });
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetByGenresAsync(new List<int> { (int)BookGenreType.Horror });

        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetRandomAsync_ReturnsRequestedCount()
    {
        using var ctx = CreateContext(nameof(GetRandomAsync_ReturnsRequestedCount));
        ctx.Books.AddRange(
            MakeBook(1, "Book A", "Auth A"),
            MakeBook(2, "Book B", "Auth B"),
            MakeBook(3, "Book C", "Auth C"),
            MakeBook(4, "Book D", "Auth D"),
            MakeBook(5, "Book E", "Auth E")
        );
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetRandomAsync(3);

        Assert.AreEqual(3, result.Count());
    }

    [TestMethod]
    public async Task GetRandomAsync_FewerBooksThanCount_ReturnsAllBooks()
    {
        using var ctx = CreateContext(nameof(GetRandomAsync_FewerBooksThanCount_ReturnsAllBooks));
        ctx.Books.AddRange(MakeBook(1, "Book A", "Auth A"), MakeBook(2, "Book B", "Auth B"));
        await ctx.SaveChangesAsync();

        var repo = new BookRepository(ctx);
        var result = await repo.GetRandomAsync(10);

        Assert.AreEqual(2, result.Count());
    }
}
