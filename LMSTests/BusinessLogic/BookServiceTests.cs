using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.Services;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

[TestClass]
public class BookServiceTests
{
    private Mock<IMapper> _mapper = null!;
    private Mock<IBookRepository> _repo = null!;
    private BookService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repo = new Mock<IBookRepository>();
        _service = new BookService(_mapper.Object, _repo.Object);
    }

    private static Book MakeBook(int isbn, string title = "Book", string author = "Author")
        => new() { ISBN = isbn, Title = title, Author = author, Description = "Desc", Count = 5, Status = BookStatus.InStock };

    private static BookReadDto MakeDto(int isbn, string title = "Book")
        => new(isbn, title, "Author", "Desc");

    // ── GetAllWithFiltersAsync ────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAllWithFiltersAsync_CallsRepositoryAndMapsDtos()
    {
        var books = new List<Book> { MakeBook(1001, "Clean Code") };
        var dtos = new List<BookReadDto> { MakeDto(1001, "Clean Code") };
        _repo.Setup(r => r.GetAllWithFiltersAsync("Clean", null, null)).ReturnsAsync(books);
        _mapper.Setup(m => m.Map<IEnumerable<BookReadDto>>(books)).Returns(dtos);

        var result = await _service.GetAllWithFiltersAsync("Clean", null, null);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Clean Code", result.First().Title);
    }

    [TestMethod]
    public async Task GetAllWithFiltersAsync_NoFilters_ReturnsAllBooks()
    {
        var books = new List<Book> { MakeBook(1), MakeBook(2), MakeBook(3) };
        var dtos = new List<BookReadDto> { MakeDto(1), MakeDto(2), MakeDto(3) };
        _repo.Setup(r => r.GetAllWithFiltersAsync(null, null, null)).ReturnsAsync(books);
        _mapper.Setup(m => m.Map<IEnumerable<BookReadDto>>(books)).Returns(dtos);

        var result = await _service.GetAllWithFiltersAsync(null, null, null);

        Assert.AreEqual(3, result.Count());
    }

    // ── GetBookDetailsAsync ───────────────────────────────────────────────────

    [TestMethod]
    public async Task GetBookDetailsAsync_BookNotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(9999, It.IsAny<IncludeBehavior>(), It.IsAny<Func<IQueryable<Book>, IQueryable<Book>>?>()))
            .ReturnsAsync((Book?)null);

        var result = await _service.GetBookDetailsAsync(9999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetBookDetailsAsync_BookFound_ReturnsEnrichedDto()
    {
        var book = MakeBook(1001, "Dune");
        book.Loans = new List<LoanBookRelation> { new() { BookISBN = 1001, Count = 2 } };
        var baseDto = MakeDto(1001, "Dune");
        _repo.Setup(r => r.GetByIdAsync(1001, It.IsAny<IncludeBehavior>(), It.IsAny<Func<IQueryable<Book>, IQueryable<Book>>?>()))
            .ReturnsAsync(book);
        _mapper.Setup(m => m.Map<BookReadDto>(book)).Returns(baseDto);

        var result = await _service.GetBookDetailsAsync(1001);

        Assert.IsNotNull(result);
        Assert.AreEqual(14, result.LoanDurationDays);
        Assert.IsTrue(result.CanBeReserved); // Count=5, Loaned=2, Available=3 > 0
    }

    [TestMethod]
    public async Task GetBookDetailsAsync_AllCopiesBorrowed_CanBeReservedIsFalse()
    {
        var book = MakeBook(1002, "Sold Out");
        book.Count = 2;
        book.Loans = new List<LoanBookRelation> { new() { BookISBN = 1002, Count = 2 } };
        var baseDto = new BookReadDto(1002, "Sold Out", "Author", "Desc", Count: 2);
        _repo.Setup(r => r.GetByIdAsync(1002, It.IsAny<IncludeBehavior>(), It.IsAny<Func<IQueryable<Book>, IQueryable<Book>>?>()))
            .ReturnsAsync(book);
        _mapper.Setup(m => m.Map<BookReadDto>(book)).Returns(baseDto);

        var result = await _service.GetBookDetailsAsync(1002);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.CanBeReserved);
    }

    // ── GetBooksByGenresAsync ─────────────────────────────────────────────────

    [TestMethod]
    public async Task GetBooksByGenresAsync_CallsRepositoryAndMapsDtos()
    {
        var genres = new List<int> { (int)BookGenreType.Fantasy };
        var books = new List<Book> { MakeBook(1, "Fantasy Book") };
        var dtos = new List<BookReadDto> { MakeDto(1, "Fantasy Book") };
        _repo.Setup(r => r.GetByGenresAsync(genres)).ReturnsAsync(books);
        _mapper.Setup(m => m.Map<IEnumerable<BookReadDto>>(books)).Returns(dtos);

        var result = await _service.GetBooksByGenresAsync(genres);

        Assert.AreEqual(1, result.Count());
        _repo.Verify(r => r.GetByGenresAsync(genres), Times.Once);
    }

    // ── GetRandomBooksAsync ───────────────────────────────────────────────────

    [TestMethod]
    public async Task GetRandomBooksAsync_CallsRepositoryWithCount()
    {
        var books = new List<Book> { MakeBook(1), MakeBook(2), MakeBook(3), MakeBook(4) };
        var dtos = books.Select(b => MakeDto(b.ISBN)).ToList();
        _repo.Setup(r => r.GetRandomAsync(4)).ReturnsAsync(books);
        _mapper.Setup(m => m.Map<IEnumerable<BookReadDto>>(books)).Returns(dtos);

        var result = await _service.GetRandomBooksAsync(4);

        Assert.AreEqual(4, result.Count());
        _repo.Verify(r => r.GetRandomAsync(4), Times.Once);
    }
}
