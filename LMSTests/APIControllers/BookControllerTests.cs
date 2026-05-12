using BusinessLogic.DTOs.Book;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMSTests.APIControllers;

[TestClass]
public class BookControllerTests
{
    private Mock<IBookService> _bookService = null!;
    private BookController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _bookService = new Mock<IBookService>();
        _controller = new BookController(_bookService.Object);
    }

    private static BookReadDto MakeDto(int isbn = 1001, string title = "Book", BookStatus status = BookStatus.InStock)
        => new(isbn, title, "Author", "Desc", null, 5, null, status);

    // ── Get ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Get_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Get(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Get_BookNotFound_ReturnsNotFound()
    {
        _bookService.Setup(s => s.GetByIdAsync(1001, It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync((BookReadDto?)null);

        var result = await _controller.Get(1001);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ValidId_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _bookService.Setup(s => s.GetByIdAsync(1001, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dto);

        var result = await _controller.Get(1001);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAll_ReturnsOkWithBooks()
    {
        var dtos = new List<BookReadDto> { MakeDto(1001), MakeDto(1002, "Book B") };
        _bookService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dtos);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    // ── SearchBooks ───────────────────────────────────────────────────────────

    [TestMethod]
    public async Task SearchBooks_InvalidBranchId_ReturnsBadRequest()
    {
        var result = await _controller.SearchBooks(null, null, -1);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task SearchBooks_ZeroBranchId_ReturnsBadRequest()
    {
        var result = await _controller.SearchBooks(null, null, 0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task SearchBooks_WithTitleFilter_ReturnsOk()
    {
        var dtos = new List<BookReadDto> { MakeDto(1001, "Clean Code") };
        _bookService.Setup(s => s.GetAllWithFiltersAsync("Clean", null, null)).ReturnsAsync(dtos);

        var result = await _controller.SearchBooks("Clean", null, null);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task SearchBooks_NoFilters_ReturnsOkWithAllBooks()
    {
        var dtos = new List<BookReadDto> { MakeDto(1001), MakeDto(1002) };
        _bookService.Setup(s => s.GetAllWithFiltersAsync(null, null, null)).ReturnsAsync(dtos);

        var result = await _controller.SearchBooks(null, null, null);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    // ── Post ──────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Post_Success_ReturnsOkTrue()
    {
        _bookService.Setup(s => s.CreateAsync(It.IsAny<BookCreateDto>())).ReturnsAsync(true);

        var result = await _controller.Post(new BookCreateDto(1001, "New Book", "Author", "Desc"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── Put ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Put_Success_ReturnsOkTrue()
    {
        _bookService.Setup(s => s.UpdateAsync(It.IsAny<BookUpdateDto>())).ReturnsAsync(true);

        var result = await _controller.Put(new BookUpdateDto(1001, "Updated Book", "Author", "Desc"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── GetStats ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetStats_ReturnsOkWithCounts()
    {
        var dtos = new List<BookReadDto>
        {
            MakeDto(1001, "A", BookStatus.InStock),
            MakeDto(1002, "B", BookStatus.OutOfStock),
            MakeDto(1003, "C", BookStatus.InStock)
        };
        _bookService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dtos);

        var result = await _controller.GetStats();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        var stats = ok.Value;
        Assert.IsNotNull(stats);
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Delete_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Delete(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Delete_Success_ReturnsOkTrue()
    {
        _bookService.Setup(s => s.DeleteAsync(1001)).ReturnsAsync(true);

        var result = await _controller.Delete(1001);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── GetDetails ────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetDetails_ZeroISBN_ReturnsBadRequest()
    {
        var result = await _controller.GetDetails(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetDetails_NegativeISBN_ReturnsBadRequest()
    {
        var result = await _controller.GetDetails(-1);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetDetails_BookNotFound_ReturnsNotFound()
    {
        _bookService.Setup(s => s.GetBookDetailsAsync(9999)).ReturnsAsync((BookReadDto?)null);

        var result = await _controller.GetDetails(9999);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetDetails_BookFound_ReturnsOkWithDto()
    {
        var dto = MakeDto(1001, "Dune");
        _bookService.Setup(s => s.GetBookDetailsAsync(1001)).ReturnsAsync(dto);

        var result = await _controller.GetDetails(1001);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetByGenres ───────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetByGenres_EmptyGenreList_ReturnsRandomBooks()
    {
        var randomBooks = new List<BookReadDto> { MakeDto(1), MakeDto(2), MakeDto(3), MakeDto(4) };
        _bookService.Setup(s => s.GetRandomBooksAsync(4)).ReturnsAsync(randomBooks);

        var result = await _controller.GetByGenres(new List<int>());

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        _bookService.Verify(s => s.GetRandomBooksAsync(4), Times.Once);
    }

    [TestMethod]
    public async Task GetByGenres_WithGenres_ReturnsFilteredBooks()
    {
        var genres = new List<int> { (int)BookGenreType.Fantasy };
        var books = new List<BookReadDto> { MakeDto(1001, "Fantasy Book") };
        _bookService.Setup(s => s.GetBooksByGenresAsync(genres)).ReturnsAsync(books);

        var result = await _controller.GetByGenres(genres);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        _bookService.Verify(s => s.GetBooksByGenresAsync(genres), Times.Once);
    }
}
