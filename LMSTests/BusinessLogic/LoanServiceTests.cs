using AutoMapper;
using BusinessLogic.DTOs.Book;
using BusinessLogic.DTOs.Loan;
using BusinessLogic.Services;
using Moq;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Repositories.Abstract;
using Repository.Tables;

namespace LMSTests.BusinessLogic;

[TestClass]
public class LoanServiceTests
{
    private Mock<IMapper> _mapper = null!;
    private Mock<ILoanRepository> _loanRepo = null!;
    private Mock<ILoanBookRelationRepository> _lbrRepo = null!;
    private LoanService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _loanRepo = new Mock<ILoanRepository>();
        _lbrRepo = new Mock<ILoanBookRelationRepository>();
        _service = new LoanService(_mapper.Object, _loanRepo.Object, _lbrRepo.Object);
    }

    // ── CreateReservationAsync ────────────────────────────────────────────────

    [TestMethod]
    public async Task CreateReservationAsync_UserHasUnpaidFines_ThrowsException()
    {
        _loanRepo.Setup(r => r.HasUnpaidFinesAsync(1)).ReturnsAsync(true);
        var dto = new LoanCreateDto("Alice", new List<BookRelationDto> { new(1001, 1) });

        bool threw = false;
        try { await _service.CreateReservationAsync(dto, 1, DateTime.UtcNow.AddDays(3)); }
        catch (Exception) { threw = true; }

        Assert.IsTrue(threw);
    }

    [TestMethod]
    public async Task CreateReservationAsync_ValidData_ReturnsTrue()
    {
        _loanRepo.Setup(r => r.HasUnpaidFinesAsync(1)).ReturnsAsync(false);
        var dto = new LoanCreateDto("Alice", new List<BookRelationDto> { new(1001, 1) });
        var loan = new Loan { Id = 10, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3), Status = LoanStatus.Pending };
        _mapper.Setup(m => m.Map<Loan>(dto)).Returns(loan);
        _loanRepo.Setup(r => r.AddAsync(loan)).Returns(Task.CompletedTask);
        _loanRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _lbrRepo.Setup(r => r.AddAsync(It.IsAny<LoanBookRelation>())).Returns(Task.CompletedTask);

        var result = await _service.CreateReservationAsync(dto, 1, DateTime.UtcNow.AddDays(3));

        Assert.IsTrue(result);
        _loanRepo.Verify(r => r.AddAsync(loan), Times.Once);
    }

    // ── GetActiveReservationsAsync ────────────────────────────────────────────

    [TestMethod]
    public async Task GetActiveReservationsAsync_ReturnsMappedDtos()
    {
        var loans = new List<Loan>
        {
            new() { Id = 1, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), Status = LoanStatus.Active }
        };
        var dtos = new List<LoanReadDto> { new(1, "Alice", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(14), LoanStatus.Active, null) };
        _loanRepo.Setup(r => r.GetByStatusAsync(LoanStatus.Active)).ReturnsAsync(loans);
        _mapper.Setup(m => m.Map<IEnumerable<LoanReadDto>>(loans)).Returns(dtos);

        var result = await _service.GetActiveReservationsAsync();

        Assert.AreEqual(1, result.Count());
    }

    // ── ApproveAndActivateLoanAsync ───────────────────────────────────────────

    [TestMethod]
    public async Task ApproveAndActivateLoanAsync_LoanNotFound_ThrowsException()
    {
        _loanRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync((Loan?)null);

        bool threw = false;
        try { await _service.ApproveAndActivateLoanAsync(99); }
        catch (Exception) { threw = true; }

        Assert.IsTrue(threw);
    }

    [TestMethod]
    public async Task ApproveAndActivateLoanAsync_LoanNotActive_ThrowsException()
    {
        var loan = new Loan { Id = 1, Status = LoanStatus.Pending, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) };
        _loanRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(loan);

        bool threw = false;
        try { await _service.ApproveAndActivateLoanAsync(1); }
        catch (Exception) { threw = true; }

        Assert.IsTrue(threw);
    }

    [TestMethod]
    public async Task ApproveAndActivateLoanAsync_ActiveLoan_ReturnsMappedDto()
    {
        var loan = new Loan { Id = 1, Status = LoanStatus.Active, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) };
        var dto = new LoanReadDto(1, "Alice", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(14), LoanStatus.Active, null);
        _loanRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<IncludeBehavior>(), null)).ReturnsAsync(loan);
        _loanRepo.Setup(r => r.Update(loan));
        _loanRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<LoanReadDto>(loan)).Returns(dto);

        var result = await _service.ApproveAndActivateLoanAsync(1);

        Assert.IsNotNull(result);
        _loanRepo.Verify(r => r.Update(loan), Times.Once);
        _loanRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    // ── GetLoansByUserIdAsync ─────────────────────────────────────────────────

    [TestMethod]
    public async Task GetLoansByUserIdAsync_ReturnsMappedUserLoans()
    {
        var loans = new List<Loan>
        {
            new() { Id = 1, UserId = 5, IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14), Status = LoanStatus.Active }
        };
        var dtos = new List<LoanReadDto> { new(1, "Bob", null, DateTime.UtcNow, DateTime.UtcNow.AddDays(14), LoanStatus.Active, null) };
        _loanRepo.Setup(r => r.GetLoansByUserIdAsync(5)).ReturnsAsync(loans);
        _mapper.Setup(m => m.Map<IEnumerable<LoanReadDto>>(loans)).Returns(dtos);

        var result = await _service.GetLoansByUserIdAsync(5);

        Assert.AreEqual(1, result.Count());
    }

    [TestMethod]
    public async Task GetLoansByUserIdAsync_NoLoans_ReturnsEmpty()
    {
        _loanRepo.Setup(r => r.GetLoansByUserIdAsync(99)).ReturnsAsync(new List<Loan>());
        _mapper.Setup(m => m.Map<IEnumerable<LoanReadDto>>(It.IsAny<IEnumerable<Loan>>()))
            .Returns(new List<LoanReadDto>());

        var result = await _service.GetLoansByUserIdAsync(99);

        Assert.AreEqual(0, result.Count());
    }
}
