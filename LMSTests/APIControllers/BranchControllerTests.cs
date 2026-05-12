using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Enums.Behaviors;

namespace LMSTests.APIControllers;

[TestClass]
public class BranchControllerTests
{
    private Mock<IBranchService> _branchService = null!;
    private BranchController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _branchService = new Mock<IBranchService>();
        _controller = new BranchController(_branchService.Object);
    }

    private static BranchReadDto MakeDto(int id = 1, string name = "Central")
        => new(id, name, "Main St", true, null, null);

    // ── Get ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Get_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Get(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Get_BranchNotFound_ReturnsNotFound()
    {
        _branchService.Setup(s => s.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync((BranchReadDto?)null);

        var result = await _controller.Get(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ValidId_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _branchService.Setup(s => s.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dto);

        var result = await _controller.Get(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAll_ReturnsOkWithBranches()
    {
        var dtos = new List<BranchReadDto> { MakeDto(1), MakeDto(2, "North") };
        _branchService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dtos);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetAll_EmptyBranches_ReturnsOkEmptyList()
    {
        _branchService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(new List<BranchReadDto>());

        var result = await _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    // ── Post ──────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Post_Success_ReturnsOkTrue()
    {
        _branchService.Setup(s => s.CreateAsync(It.IsAny<BranchCreateDto>())).ReturnsAsync(true);

        var result = await _controller.Post(new BranchCreateDto("New Branch", "New Addr", true));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Post_Failure_ReturnsOkFalse()
    {
        _branchService.Setup(s => s.CreateAsync(It.IsAny<BranchCreateDto>())).ReturnsAsync(false);

        var result = await _controller.Post(new BranchCreateDto("X", "Y", true));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }

    // ── Put ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Put_Success_ReturnsOkTrue()
    {
        _branchService.Setup(s => s.UpdateAsync(It.IsAny<BranchUpdateDto>())).ReturnsAsync(true);

        var result = await _controller.Put(new BranchUpdateDto(1, "Updated", "New Addr", false));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
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
        _branchService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Delete_ServiceThrows_ReturnsBadRequest()
    {
        _branchService.Setup(s => s.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _controller.Delete(1);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
