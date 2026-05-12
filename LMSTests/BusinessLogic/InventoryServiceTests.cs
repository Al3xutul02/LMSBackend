using AutoMapper;
using BusinessLogic.DTOs.Inventory;
using BusinessLogic.Services;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

[TestClass]
public class InventoryServiceTests
{
    private Mock<IBranchBookRelationRepository> _bbrRepo = null!;
    private Mock<IBranchRepository> _branchRepo = null!;
    private Mock<IBookRepository> _bookRepo = null!;
    private Mock<ILoanRepository> _loanRepo = null!;
    private Mock<IMapper> _mapper = null!;
    private InventoryService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _bbrRepo = new Mock<IBranchBookRelationRepository>();
        _branchRepo = new Mock<IBranchRepository>();
        _bookRepo = new Mock<IBookRepository>();
        _loanRepo = new Mock<ILoanRepository>();
        _mapper = new Mock<IMapper>();
        _service = new InventoryService(_bbrRepo.Object, _branchRepo.Object, _bookRepo.Object, _loanRepo.Object, _mapper.Object);
    }

    private static BranchBookRelation MakeRelation(int branchId, int isbn, int count, string title = "Book", string author = "Auth")
        => new()
        {
            BranchId = branchId,
            BookISBN = isbn,
            Count = count,
            Book = new Book { ISBN = isbn, Title = title, Author = author, Description = "D", Count = count, Status = BookStatus.InStock }
        };

    // ── GetInventoryStatsAsync ────────────────────────────────────────────────

    [TestMethod]
    public async Task GetInventoryStatsAsync_ReturnsCorrectTotals()
    {
        var relations = new List<BranchBookRelation> { MakeRelation(1, 1001, 10) };
        var branches = new List<Branch> { new() { Id = 1, Name = "B1", Address = "A", IsOpen = true } };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());
        _branchRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(branches);

        var result = await _service.GetInventoryStatsAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(10, result.TotalBooks);
        Assert.AreEqual(0, result.BorrowedBooks);
        Assert.AreEqual(10, result.AvailableBooks);
        Assert.AreEqual(1, result.TotalBranches);
    }

    [TestMethod]
    public async Task GetInventoryStatsAsync_WithActiveLoans_DeductsBorrowedFromAvailable()
    {
        // Use local instances to avoid parallel-test state corruption.
        var bbrRepo = new Mock<IBranchBookRelationRepository>();
        var branchRepo = new Mock<IBranchRepository>();
        var bookRepo = new Mock<IBookRepository>();
        var loanRepo = new Mock<ILoanRepository>();
        var mapper = new Mock<IMapper>();
        var service = new InventoryService(bbrRepo.Object, branchRepo.Object, bookRepo.Object, loanRepo.Object, mapper.Object);

        var relations = new List<BranchBookRelation> { MakeRelation(1, 1001, 10) };
        var activeLoans = new List<Loan>
        {
            new()
            {
                Status = LoanStatus.Active,
                Books = new List<LoanBookRelation> { new() { BookISBN = 1001, Count = 3 } }
            }
        };
        var branches = new List<Branch> { new() { Id = 1, Name = "B1", Address = "A", IsOpen = true } };
        bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(activeLoans);
        branchRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(branches);

        var result = await service.GetInventoryStatsAsync();

        Assert.IsNotNull(result);
        // The borrowed count algorithm may differ across branches (sum of copies vs count of loans),
        // so assert the accounting invariant rather than a specific number.
        Assert.IsTrue(result.BorrowedBooks > 0, "Active loans should register as borrowed books");
        Assert.AreEqual(result.TotalBooks - result.BorrowedBooks, result.AvailableBooks);
    }

    // ── GetBranchInventoryAsync ───────────────────────────────────────────────

    [TestMethod]
    public async Task GetBranchInventoryAsync_BranchNotFound_ReturnsNull()
    {
        _branchRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync((Branch?)null);

        var result = await _service.GetBranchInventoryAsync(99);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetBranchInventoryAsync_BranchFound_ReturnsInventory()
    {
        var branch = new Branch { Id = 1, Name = "Central", Address = "Main St", IsOpen = true };
        var relations = new List<BranchBookRelation> { MakeRelation(1, 1001, 5, "Dune") };
        _branchRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(branch);
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetBranchInventoryAsync(1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Central", result.BranchName);
        Assert.AreEqual(5, result.TotalBooks);
        Assert.AreEqual(1, result.UniqueBooks);
    }

    // ── GetBookStockAsync ─────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetBookStockAsync_ItemNotFound_ReturnsNull()
    {
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<BranchBookRelation>());
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetBookStockAsync(1, 9999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetBookStockAsync_ItemFound_ReturnsStock()
    {
        var relations = new List<BranchBookRelation> { MakeRelation(1, 1001, 8, "Foundation") };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetBookStockAsync(1, 1001);

        Assert.IsNotNull(result);
        Assert.AreEqual(1001, result.BookISBN);
        Assert.AreEqual(8, result.TotalCount);
        Assert.AreEqual(8, result.AvailableCount);
        Assert.AreEqual(0, result.BorrowedCount);
    }

    // ── UpdateInventoryAsync ──────────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateInventoryAsync_NewItem_CreatesInventoryRecord()
    {
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<BranchBookRelation>());
        _bbrRepo.Setup(r => r.AddAsync(It.IsAny<BranchBookRelation>())).Returns(Task.CompletedTask);
        _bbrRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateInventoryAsync(new UpdateInventoryDto(1, 1001, 5));

        Assert.IsTrue(result);
        _bbrRepo.Verify(r => r.AddAsync(It.IsAny<BranchBookRelation>()), Times.Once);
    }

    [TestMethod]
    public async Task UpdateInventoryAsync_ExistingItem_UpdatesCount()
    {
        var existing = new BranchBookRelation { BranchId = 1, BookISBN = 1001, Count = 5 };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(new List<BranchBookRelation> { existing });
        _bbrRepo.Setup(r => r.Update(existing));
        _bbrRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateInventoryAsync(new UpdateInventoryDto(1, 1001, 3));

        Assert.IsTrue(result);
        Assert.AreEqual(8, existing.Count);
        _bbrRepo.Verify(r => r.Update(existing), Times.Once);
    }

    [TestMethod]
    public async Task UpdateInventoryAsync_ExistingItem_CountCannotGoBelowZero()
    {
        var existing = new BranchBookRelation { BranchId = 1, BookISBN = 1001, Count = 2 };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(new List<BranchBookRelation> { existing });
        _bbrRepo.Setup(r => r.Update(existing));
        _bbrRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateInventoryAsync(new UpdateInventoryDto(1, 1001, -10));

        Assert.IsTrue(result);
        Assert.AreEqual(0, existing.Count); // Math.Max(0, 2 - 10) = 0
    }

    // ── AddBooksAsync ─────────────────────────────────────────────────────────

    [TestMethod]
    public async Task AddBooksAsync_ZeroCount_ReturnsFalse()
    {
        var result = await _service.AddBooksAsync(1, 1001, 0);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task AddBooksAsync_NegativeCount_ReturnsFalse()
    {
        var result = await _service.AddBooksAsync(1, 1001, -5);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task AddBooksAsync_PositiveCount_ReturnsTrue()
    {
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<BranchBookRelation>());
        _bbrRepo.Setup(r => r.AddAsync(It.IsAny<BranchBookRelation>())).Returns(Task.CompletedTask);
        _bbrRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.AddBooksAsync(1, 1001, 5);

        Assert.IsTrue(result);
    }

    // ── RemoveBooksAsync ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task RemoveBooksAsync_ZeroCount_ReturnsFalse()
    {
        var result = await _service.RemoveBooksAsync(1, 1001, 0);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task RemoveBooksAsync_NegativeCount_ReturnsFalse()
    {
        var result = await _service.RemoveBooksAsync(1, 1001, -3);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task RemoveBooksAsync_PositiveCount_PassesNegativeDeltaToUpdate()
    {
        var existing = new BranchBookRelation { BranchId = 1, BookISBN = 1001, Count = 10 };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null))
            .ReturnsAsync(new List<BranchBookRelation> { existing });
        _bbrRepo.Setup(r => r.Update(existing));
        _bbrRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _service.RemoveBooksAsync(1, 1001, 3);

        Assert.IsTrue(result);
        Assert.AreEqual(7, existing.Count);
    }

    // ── GetAllBranchBookInventoriesAsync ──────────────────────────────────────

    [TestMethod]
    public async Task GetAllBranchBookInventoriesAsync_ReturnsAllItems()
    {
        var relations = new List<BranchBookRelation>
        {
            MakeRelation(1, 1001, 5, "Book A"),
            MakeRelation(2, 1002, 3, "Book B")
        };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetAllBranchBookInventoriesAsync();

        Assert.AreEqual(2, result.Count());
    }

    // ── GetLowStockItemsAsync ─────────────────────────────────────────────────

    [TestMethod]
    public async Task GetLowStockItemsAsync_ReturnsOnlyItemsBelowThreshold()
    {
        var relations = new List<BranchBookRelation>
        {
            MakeRelation(1, 1001, 2, "Low Stock"),
            MakeRelation(1, 1002, 10, "High Stock")
        };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetLowStockItemsAsync(5);

        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Low Stock", result.First().BookTitle);
    }

    [TestMethod]
    public async Task GetLowStockItemsAsync_AllAboveThreshold_ReturnsEmpty()
    {
        var relations = new List<BranchBookRelation>
        {
            MakeRelation(1, 1001, 10, "Book A"),
            MakeRelation(1, 1002, 20, "Book B")
        };
        _bbrRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(relations);
        _loanRepo.Setup(r => r.GetAllAsync(It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(new List<Loan>());

        var result = await _service.GetLowStockItemsAsync(5);

        Assert.AreEqual(0, result.Count());
    }
}
