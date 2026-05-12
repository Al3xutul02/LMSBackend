using BusinessLogic.DTOs.Login;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMSTests.APIControllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _authService = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _authService = new Mock<IAuthService>();
        _controller = new AuthController(_authService.Object);
    }

    private void SetAuthHeader(string? value)
    {
        var httpContext = new DefaultHttpContext();
        if (value != null)
            httpContext.Request.Headers["Authorization"] = value;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    // ── IsLoggedIn ────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task IsLoggedIn_NoAuthHeader_ReturnsBadRequest()
    {
        SetAuthHeader(null);

        var result = await _controller.IsLoggedIn();

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task IsLoggedIn_NotBearerScheme_ReturnsBadRequest()
    {
        SetAuthHeader("Basic dXNlcjpwYXNz");

        var result = await _controller.IsLoggedIn();

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task IsLoggedIn_ServiceReturnsTrue_ReturnsOkTrue()
    {
        SetAuthHeader("Bearer valid-token");
        _authService.Setup(s => s.IsLoggedIn("valid-token")).ReturnsAsync(true);

        var result = await _controller.IsLoggedIn();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    [TestMethod]
    public async Task IsLoggedIn_ServiceReturnsFalse_ReturnsOkFalse()
    {
        SetAuthHeader("Bearer expired-token");
        _authService.Setup(s => s.IsLoggedIn("expired-token")).ReturnsAsync(false);

        var result = await _controller.IsLoggedIn();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(false, ok.Value);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Login_EmptyUsername_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        var result = await _controller.Login(new LoginDto("", "pass"));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Login_EmptyPassword_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        var result = await _controller.Login(new LoginDto("user", ""));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Login_UserNotFound_ReturnsNotFound()
    {
        SetAuthHeader(null);
        _authService.Setup(s => s.Login(It.IsAny<LoginDto>())).ReturnsAsync((LoginResponseDto?)null);

        var result = await _controller.Login(new LoginDto("unknown", "pass"));

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsOkWithResponse()
    {
        SetAuthHeader(null);
        var response = new LoginResponseDto("jwt-token", "refresh-token");
        _authService.Setup(s => s.Login(It.IsAny<LoginDto>())).ReturnsAsync(response);

        var result = await _controller.Login(new LoginDto("alice", "password"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(response, ok.Value);
    }

    // ── Register ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Register_EmptyName_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        var result = await _controller.Register(new UserCreateDto("", "e@test.com", "pass"));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Register_EmptyEmail_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        var result = await _controller.Register(new UserCreateDto("Alice", "", "pass"));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Register_EmptyPassword_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        var result = await _controller.Register(new UserCreateDto("Alice", "a@test.com", ""));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Register_ServiceReturnsNull_ReturnsBadRequest()
    {
        SetAuthHeader(null);
        _authService.Setup(s => s.Register(It.IsAny<UserCreateDto>())).ReturnsAsync((LoginResponseDto?)null);

        var result = await _controller.Register(new UserCreateDto("Alice", "alice@test.com", "pass"));

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Register_ValidData_ReturnsOkWithResponse()
    {
        SetAuthHeader(null);
        var response = new LoginResponseDto("jwt-token", "refresh-token");
        _authService.Setup(s => s.Register(It.IsAny<UserCreateDto>())).ReturnsAsync(response);

        var result = await _controller.Register(new UserCreateDto("Alice", "alice@test.com", "password"));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(response, ok.Value);
    }

    // ── RefreshToken ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task RefreshToken_NoAuthHeader_ReturnsBadRequest()
    {
        SetAuthHeader(null);

        var result = await _controller.RefreshToken("some-refresh");

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task RefreshToken_ServiceReturnsNull_ReturnsBadRequest()
    {
        SetAuthHeader("Bearer expired-access-token");
        _authService.Setup(s => s.RefreshToken(It.IsAny<LoginResponseDto>())).ReturnsAsync((LoginResponseDto?)null);

        var result = await _controller.RefreshToken("bad-refresh");

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task RefreshToken_ValidTokens_ReturnsOkWithNewResponse()
    {
        SetAuthHeader("Bearer old-access-token");
        var newResponse = new LoginResponseDto("new-jwt", "new-refresh");
        _authService.Setup(s => s.RefreshToken(It.IsAny<LoginResponseDto>())).ReturnsAsync(newResponse);

        var result = await _controller.RefreshToken("valid-refresh");

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(newResponse, ok.Value);
    }
}
