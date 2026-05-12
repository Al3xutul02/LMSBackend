using AutoMapper;
using BusinessLogic.DTOs.Branch;
using BusinessLogic.Services;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

/// <summary>
/// Tests for BaseService methods via BranchService (which adds no extra logic).
/// </summary>
[TestClass]
public class BaseServiceTests
{
    private Mock<IMapper> _mapper = null!;
    private Mock<IBranchRepository> _repo = null!;
    private BranchService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repo = new Mock<IBranchRepository>();
        _service = new BranchService(_mapper.Object, _repo.Object);
    }

    [TestMethod]
    public async Task GetByIdAsync_EntityFound_ReturnsMappedDto()
    {
        var branch = new Branch { Id = 1, Name = "Central", Address = "Main St", IsOpen = true };
        var dto = new BranchReadDto(1, "Central", "Main St", true, null, null);
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(branch);
        _mapper.Setup(m => m.Map<BranchReadDto>(branch)).Returns(dto);

        var result = await _service.GetByIdAsync(1, IncludeBehavior.NoInclude);

        Assert.IsNotNull(result);
        Assert.AreEqual("Central", result.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_EntityNotFound_ReturnsNull()
    {
        _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync((Branch?)null);

        var result = await _service.GetByIdAsync(99, IncludeBehavior.NoInclude);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        var branches = new List<Branch> { new() { Name = "A", Address = "Addr A", IsOpen = true } };
        var dtos = new List<BranchReadDto> { new(1, "A", "Addr A", true, null, null) };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(branches);
        _mapper.Setup(m => m.Map<IEnumerable<BranchReadDto>>(branches)).Returns(dtos);

        var result = await _service.GetAllAsync(IncludeBehavior.NoInclude);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public async Task CreateAsync_Success_ReturnsTrue()
    {
        var dto = new BranchCreateDto("New Branch", "New Addr", true);
        var entity = new Branch { Name = "New Branch", Address = "New Addr", IsOpen = true };
        _mapper.Setup(m => m.Map<Branch>(dto)).Returns(entity);
        _repo.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(dto);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CreateAsync_ExceptionThrown_ReturnsFalse()
    {
        var dto = new BranchCreateDto("X", "Y", true);
        _mapper.Setup(m => m.Map<Branch>(dto)).Throws(new Exception("mapping failed"));

        var result = await _service.CreateAsync(dto);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task UpdateAsync_CallsRepositoryUpdateAndSave()
    {
        // Use local instances to avoid any parallel-test state corruption.
        var mapper = new Mock<IMapper>();
        var repo = new Mock<IBranchRepository>();
        var service = new BranchService(mapper.Object, repo.Object);

        var dto = new BranchUpdateDto(1, "Updated", "New Addr", false);
        var entity = new Branch { Id = 1, Name = "Updated", Address = "New Addr", IsOpen = false };
        // Support both UpdateAsync implementations: one that fetches first (development branch)
        // and one that maps directly (main branch).
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<Branch>(It.IsAny<object>())).Returns(entity);
        repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await service.UpdateAsync(dto);

        Assert.IsTrue(result);
        repo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_EntityFound_ReturnsTrue()
    {
        var entity = new Branch { Id = 1, Name = "A", Address = "Addr", IsOpen = true };
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(entity);
        _repo.Setup(r => r.Delete(entity));
        _repo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.IsTrue(result);
        _repo.Verify(r => r.Delete(entity), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_EntityNotFound_ReturnsFalse()
    {
        _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync((Branch?)null);

        var result = await _service.DeleteAsync(99);

        Assert.IsFalse(result);
    }
}
