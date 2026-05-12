using BusinessLogic.DTOs.Fine;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMSTests.APIControllers;

[TestClass]
public class FineControllerTests
{
    private Mock<IFineService> _fineService = null!;
    private FineController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _fineService = new Mock<IFineService>();
        _controller = new FineController(_fineService.Object);
    }

    private static FineReadDto MakeDto(int id = 1) => new(id, 10, 50, FineStatus.Unpaid);

    // ── Get ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Get_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Get(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Get_FineNotFound_ReturnsNotFound()
    {
        _fineService.Setup(s => s.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync((FineReadDto?)null);

        var result = await _controller.Get(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ValidId_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _fineService.Setup(s => s.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dto);

        var result = await _controller.Get(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAll_ReturnsOkWithFines()
    {
        var dtos = new List<FineReadDto> { MakeDto(1), MakeDto(2) };
        _fineService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dtos);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetAll_EmptyFines_ReturnsOkEmptyList()
    {
        _fineService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(new List<FineReadDto>());

        var result = await _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    // ── Post ──────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Post_Success_ReturnsOkTrue()
    {
        _fineService.Setup(s => s.CreateAsync(It.IsAny<FineCreateDto>())).ReturnsAsync(true);

        var result = await _controller.Post(new FineCreateDto(1, 50));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Post_Failure_ReturnsOkFalse()
    {
        _fineService.Setup(s => s.CreateAsync(It.IsAny<FineCreateDto>())).ReturnsAsync(false);

        var result = await _controller.Post(new FineCreateDto(1, 50));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }

    // ── Put ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Put_Success_ReturnsOkTrue()
    {
        _fineService.Setup(s => s.UpdateAsync(It.IsAny<FineUpdateDto>())).ReturnsAsync(true);

        var result = await _controller.Put(new FineUpdateDto(1, 1, 50, FineStatus.Paid));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Put_ServiceThrows_ReturnsBadRequest()
    {
        _fineService.Setup(s => s.UpdateAsync(It.IsAny<FineUpdateDto>())).ThrowsAsync(new Exception("DB error"));

        var result = await _controller.Put(new FineUpdateDto(1, 1, 50, FineStatus.Paid));

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
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
        _fineService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Delete_NotFound_ReturnsOkFalse()
    {
        _fineService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }
}
