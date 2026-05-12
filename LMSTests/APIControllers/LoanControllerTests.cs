using BusinessLogic.DTOs.Book;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMSTests.APIControllers;

[TestClass]
public class LoanControllerTests
{
    private Mock<ILoanService> _loanService = null!;
    private LoanController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _loanService = new Mock<ILoanService>();
        _controller = new LoanController(_loanService.Object);
    }

    private static LoanReadDto MakeDto(int id = 1, LoanStatus status = LoanStatus.Active)
        => new(id, "Alice", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(14), status, null);

    // ── Get ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Get_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Get(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Get_LoanNotFound_ReturnsNotFound()
    {
        _loanService.Setup(s => s.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync((LoanReadDto?)null);

        var result = await _controller.Get(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ValidId_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _loanService.Setup(s => s.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dto);

        var result = await _controller.Get(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAll_ReturnsOkWithLoans()
    {
        var dtos = new List<LoanReadDto> { MakeDto(1), MakeDto(2) };
        _loanService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(dtos);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    // ── Reserve ───────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Reserve_ServiceReturnsTrue_ReturnsOkTrue()
    {
        var dto = new LoanCreateDto("Alice", new List<BookRelationDto> { new(1001, 1) });
        _loanService.Setup(s => s.CreateReservationAsync(dto, 1, It.IsAny<DateTime>())).ReturnsAsync(true);

        var result = await _controller.Reserve(dto, DateTime.UtcNow.AddDays(3), 1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Reserve_ServiceThrows_ReturnsBadRequest()
    {
        var dto = new LoanCreateDto("Alice", new List<BookRelationDto> { new(1001, 1) });
        _loanService.Setup(s => s.CreateReservationAsync(It.IsAny<LoanCreateDto>(), It.IsAny<int>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Unpaid fines"));

        var result = await _controller.Reserve(dto, DateTime.UtcNow.AddDays(3), 1);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    // ── Put ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Put_ServiceSucceeds_ReturnsOk()
    {
        _loanService.Setup(s => s.GetActiveReservationsAsync()).ReturnsAsync(new List<LoanReadDto>());
        _loanService.Setup(s => s.UpdateAsync(It.IsAny<LoanUpdateDto>())).ReturnsAsync(true);

        var result = await _controller.Put(new LoanUpdateDto(1));

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task Put_ServiceThrows_ReturnsBadRequest()
    {
        // Use local instances to avoid parallel-test state corruption.
        // Mock both possible service methods so this works regardless of which
        // Put() implementation (GetActiveReservationsAsync vs UpdateAsync) is active.
        var localService = new Mock<ILoanService>();
        var localController = new LoanController(localService.Object);
        localService.Setup(s => s.GetActiveReservationsAsync()).ThrowsAsync(new Exception("DB error"));
        localService.Setup(s => s.UpdateAsync(It.IsAny<LoanUpdateDto>())).ThrowsAsync(new Exception("DB error"));

        var result = await localController.Put(new LoanUpdateDto(1));

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    // ── Delete (ApproveAndActivate) ───────────────────────────────────────────

    [TestMethod]
    public async Task Delete_Success_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _loanService.Setup(s => s.ApproveAndActivateLoanAsync(1)).ReturnsAsync(dto);

        var result = await _controller.Delete(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    [TestMethod]
    public async Task Delete_ServiceThrows_ReturnsBadRequest()
    {
        _loanService.Setup(s => s.ApproveAndActivateLoanAsync(99)).ThrowsAsync(new Exception("Not found"));

        var result = await _controller.Delete(99);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    // ── GetUserLoans ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetUserLoans_ReturnsOkWithUserLoans()
    {
        var dtos = new List<LoanReadDto> { MakeDto(1), MakeDto(2) };
        _loanService.Setup(s => s.GetLoansByUserIdAsync(5)).ReturnsAsync(dtos);

        var result = await _controller.GetUserLoans(5);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetUserLoans_ServiceThrows_ReturnsBadRequest()
    {
        _loanService.Setup(s => s.GetLoansByUserIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));

        var result = await _controller.GetUserLoans(1);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
