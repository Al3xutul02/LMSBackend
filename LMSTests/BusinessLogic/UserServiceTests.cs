using AutoMapper;
using BusinessLogic.DTOs.User;
using BusinessLogic.Services;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

[TestClass]
public class UserServiceTests
{
    private Mock<IMapper> _mapper = null!;
    private Mock<IUserRepository> _repo = null!;
    private UserService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repo = new Mock<IUserRepository>();
        _service = new UserService(_mapper.Object, _repo.Object);
    }

    [TestMethod]
    public async Task GetUserProfileAsync_UserNotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetUserForProfileAsync(1)).ReturnsAsync((User?)null);

        var result = await _service.GetUserProfileAsync(1);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetUserProfileAsync_UserFound_ReturnsMappedDto()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "alice@test.com", PasswordHash = "h", Role = UserRole.Reader };
        var dto = new UserReadDto(1, "Alice", "alice@test.com", UserRole.Reader, "", null, null);
        _repo.Setup(r => r.GetUserForProfileAsync(1)).ReturnsAsync(user);
        _mapper.Setup(m => m.Map<UserReadDto>(user)).Returns(dto);

        var result = await _service.GetUserProfileAsync(1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Alice", result.Name);
    }

    [TestMethod]
    public async Task UpdateNameAsync_UserNotFound_ReturnsFalse()
    {
        _repo.Setup(r => r.GetByIdAsync(99, IncludeBehavior.NoIncludes, null)).ReturnsAsync((User?)null);

        var result = await _service.UpdateNameAsync(99, "NewName");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task UpdateNameAsync_UserFound_UpdatesNameAndReturnsTrue()
    {
        var user = new User { Id = 1, Name = "Alice", Email = "a@test.com", PasswordHash = "h", Role = UserRole.Reader };
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(user);
        _repo.Setup(r => r.Update(user));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateNameAsync(1, "Alicia");

        Assert.IsTrue(result);
        Assert.AreEqual("Alicia", user.Name);
        _repo.Verify(r => r.Update(user), Times.Once);
        _repo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_MapsAndHashesPasswordBeforeSaving()
    {
        var dto = new UserCreateDto("Bob", "bob@test.com", "plaintext", UserRole.Reader);
        var mappedUser = new User { Name = "Bob", Email = "bob@test.com", PasswordHash = "plaintext", Role = UserRole.Reader };
        _mapper.Setup(m => m.Map<User>(dto)).Returns(mappedUser);
        _repo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.IsTrue(result);
        // Password should be hashed (not the original plaintext)
        Assert.AreNotEqual("plaintext", mappedUser.PasswordHash);
        Assert.IsTrue(mappedUser.PasswordHash.StartsWith("$2"));
    }

    [TestMethod]
    public async Task CreateAsync_MapperThrows_ReturnsFalse()
    {
        var dto = new UserCreateDto("Error", "err@test.com", "pass", UserRole.Reader);
        _mapper.Setup(m => m.Map<User>(dto)).Throws(new Exception("mapping error"));

        var result = await _service.CreateAsync(dto);

        Assert.IsFalse(result);
    }
}
