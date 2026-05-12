using BusinessLogic.DTOs.Login;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _repo = null!;
    private IConfiguration _config = null!;
    private AuthService _service = null!;

    private const string TestJwtKey = "super-secret-test-key-for-jwt-hmac-sha256-testing";
    private const string TestIssuer = "test-issuer";
    private const string TestAudience = "test-audience";

    [TestInitialize]
    public void Setup()
    {
        _repo = new Mock<IUserRepository>();
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = TestIssuer,
                ["Jwt:Audience"] = TestAudience,
                ["Jwt:ExpirationTimeMinutes"] = "60"
            })
            .Build();
        _service = new AuthService(_repo.Object, _config);
    }

    private static User MakeUser(string name = "Alice", string email = "alice@test.com")
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("password123", workFactor: 4);
        return new User { Id = 1, Name = name, Email = email, PasswordHash = hash, Role = UserRole.Reader };
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Login_UserNotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByUsernameAsync("ghost")).ReturnsAsync((User?)null);

        var result = await _service.Login(new LoginDto("ghost", "pass"));

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);

        var result = await _service.Login(new LoginDto("Alice", "wrongpass"));

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsLoginResponse()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.Login(new LoginDto("Alice", "password123"));

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.Token));
        Assert.IsFalse(string.IsNullOrEmpty(result.RefreshToken));
    }

    // ── Register ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task Register_EmailAlreadyExists_ReturnsNull()
    {
        _repo.Setup(r => r.EmailExistsAsync("alice@test.com")).ReturnsAsync(true);

        var result = await _service.Register(new UserCreateDto("Alice", "alice@test.com", "pass"));

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Register_NewEmail_CreatesUserAndReturnsResponse()
    {
        _repo.Setup(r => r.EmailExistsAsync("new@test.com")).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _repo.Setup(r => r.Update(It.IsAny<User>()));

        var result = await _service.Register(new UserCreateDto("NewUser", "new@test.com", "password123"));

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.Token));
        Assert.IsFalse(string.IsNullOrEmpty(result.RefreshToken));
    }

    // ── IsLoggedIn ────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task IsLoggedIn_NullToken_ReturnsFalse()
    {
        var result = await _service.IsLoggedIn(null!);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsLoggedIn_EmptyToken_ReturnsFalse()
    {
        var result = await _service.IsLoggedIn("");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsLoggedIn_InvalidToken_ReturnsFalse()
    {
        var result = await _service.IsLoggedIn("not.a.jwt");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task IsLoggedIn_ValidToken_ReturnsTrue()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var loginResponse = await _service.Login(new LoginDto("Alice", "password123"));
        Assert.IsNotNull(loginResponse);

        var result = await _service.IsLoggedIn(loginResponse.Token);

        Assert.IsTrue(result);
    }

    // ── RefreshToken ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task RefreshToken_InvalidAccessToken_ReturnsNullOrThrows()
    {
        // A completely malformed token causes the JWT library to throw rather than
        // returning null; both outcomes mean the caller gets no valid response.
        var oldResponse = new LoginResponseDto("not.a.valid.jwt", "some-refresh");

        LoginResponseDto? result = null;
        try { result = await _service.RefreshToken(oldResponse); }
        catch { result = null; }

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RefreshToken_UserNotFound_ReturnsNull()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var loginResponse = await _service.Login(new LoginDto("Alice", "password123"));
        Assert.IsNotNull(loginResponse);

        _repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync((User?)null);

        var result = await _service.RefreshToken(loginResponse);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RefreshToken_WrongRefreshToken_ReturnsNull()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var loginResponse = await _service.Login(new LoginDto("Alice", "password123"));
        Assert.IsNotNull(loginResponse);

        user.RefreshToken = "correct-token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(user);

        var badResponse = loginResponse with { RefreshToken = "wrong-token" };
        var result = await _service.RefreshToken(badResponse);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RefreshToken_ExpiredRefreshToken_ReturnsNull()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var loginResponse = await _service.Login(new LoginDto("Alice", "password123"));
        Assert.IsNotNull(loginResponse);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1); // expired
        _repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(user);

        var result = await _service.RefreshToken(loginResponse);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RefreshToken_ValidTokens_ReturnsNewLoginResponse()
    {
        var user = MakeUser();
        _repo.Setup(r => r.GetByUsernameAsync("Alice")).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        var loginResponse = await _service.Login(new LoginDto("Alice", "password123"));
        Assert.IsNotNull(loginResponse);

        user.RefreshToken = loginResponse.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _repo.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(user);

        var result = await _service.RefreshToken(loginResponse);

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.Token));
        Assert.IsFalse(string.IsNullOrEmpty(result.RefreshToken));
    }
}
