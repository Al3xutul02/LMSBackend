using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace LMSTests.APIControllers;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _userService = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _userService = new Mock<IUserService>();
        _controller = new UserController(_userService.Object);
    }

    private static UserReadDto MakeDto(int id = 1, string name = "Alice")
        => new(id, name, "alice@test.com", UserRole.Reader, "", null, null);

    // ── Get ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Get_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.Get(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Get_UserNotFound_ReturnsNotFound()
    {
        _userService.Setup(s => s.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync((UserReadDto?)null);

        var result = await _controller.Get(1);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Get_ValidId_ReturnsOkWithDto()
    {
        var dto = MakeDto();
        _userService.Setup(s => s.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dto);

        var result = await _controller.Get(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        var dtos = new List<UserReadDto> { MakeDto(1), MakeDto(2, "Bob") };
        _userService.Setup(s => s.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(dtos);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    // ── Post ──────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Post_Success_ReturnsOkTrue()
    {
        _userService.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>())).ReturnsAsync(true);

        var result = await _controller.Post(new UserCreateDto("New", "new@test.com", "pass"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Post_Failure_ReturnsOkFalse()
    {
        _userService.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>())).ReturnsAsync(false);

        var result = await _controller.Post(new UserCreateDto("New", "new@test.com", "pass"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }

    // ── Put ───────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Put_Success_ReturnsOkTrue()
    {
        _userService.Setup(s => s.UpdateAsync(It.IsAny<UserUpdateDto>())).ReturnsAsync(true);

        var result = await _controller.Put(new UserUpdateDto(1, "Updated", "u@test.com", "pass"));

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
        _userService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task Delete_NotFound_ReturnsOkFalse()
    {
        _userService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }

    // ── GetMyProfile ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetMyProfile_UserNotFound_ReturnsNotFound()
    {
        _userService.Setup(s => s.GetUserProfileAsync(99)).ReturnsAsync((UserReadDto?)null);

        var result = await _controller.GetMyProfile(99);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetMyProfile_UserFound_ReturnsOkWithProfile()
    {
        var dto = MakeDto();
        _userService.Setup(s => s.GetUserProfileAsync(1)).ReturnsAsync(dto);

        var result = await _controller.GetMyProfile(1);

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(dto, ok.Value);
    }

    // ── UpdateName ────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateName_EmptyName_ReturnsBadRequest()
    {
        var result = await _controller.UpdateName(1, "");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateName_WhitespaceName_ReturnsBadRequest()
    {
        var result = await _controller.UpdateName(1, "   ");
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateName_UserNotFound_ReturnsNotFound()
    {
        _userService.Setup(s => s.UpdateNameAsync(99, "NewName")).ReturnsAsync(false);

        var result = await _controller.UpdateName(99, "NewName");

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task UpdateName_Success_ReturnsOkTrue()
    {
        _userService.Setup(s => s.UpdateNameAsync(1, "Alice2")).ReturnsAsync(true);

        var result = await _controller.UpdateName(1, "Alice2");

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }
}
