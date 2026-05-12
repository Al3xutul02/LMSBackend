using BusinessLogic.DTOs.Inventory;
using BusinessLogic.Services.Abstract;
using LMS_Backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMSTests.APIControllers;

[TestClass]
public class InventoryControllerTests
{
    private Mock<IInventoryService> _inventoryService = null!;
    private InventoryController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _inventoryService = new Mock<IInventoryService>();
        _controller = new InventoryController(_inventoryService.Object);
    }

    private static BranchBookStockDto MakeStock(int isbn = 1001, int available = 5)
        => new(isbn, "Book", "Author", available, 10, 5);

    private static BranchInventoryDto MakeBranchInventory(int branchId = 1)
        => new(branchId, "Central", 10, 1, new List<BranchBookStockDto> { MakeStock() });

    // ── GetStats ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetStats_ServiceReturnsNull_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.GetInventoryStatsAsync()).ReturnsAsync((InventoryStatsDto?)null);

        var result = await _controller.GetStats();

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetStats_ServiceReturnsStats_ReturnsOk()
    {
        var stats = new InventoryStatsDto(100, 80, 20, 3, null);
        _inventoryService.Setup(s => s.GetInventoryStatsAsync()).ReturnsAsync(stats);

        var result = await _controller.GetStats();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(stats, ok.Value);
    }

    [TestMethod]
    public async Task GetStats_ServiceThrows_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.GetInventoryStatsAsync()).ThrowsAsync(new Exception("error"));

        var result = await _controller.GetStats();

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    // ── GetBranchInventory ────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetBranchInventory_ZeroId_ReturnsBadRequest()
    {
        var result = await _controller.GetBranchInventory(0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetBranchInventory_BranchNotFound_ReturnsNotFound()
    {
        _inventoryService.Setup(s => s.GetBranchInventoryAsync(99)).ReturnsAsync((BranchInventoryDto?)null);

        var result = await _controller.GetBranchInventory(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetBranchInventory_Found_ReturnsOk()
    {
        var inv = MakeBranchInventory();
        _inventoryService.Setup(s => s.GetBranchInventoryAsync(1)).ReturnsAsync(inv);

        var result = await _controller.GetBranchInventory(1);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(inv, ok.Value);
    }

    // ── GetBookStock ──────────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetBookStock_ZeroBranchId_ReturnsBadRequest()
    {
        var result = await _controller.GetBookStock(0, 1001);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetBookStock_ZeroBookISBN_ReturnsBadRequest()
    {
        var result = await _controller.GetBookStock(1, 0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task GetBookStock_ItemNotFound_ReturnsNotFound()
    {
        _inventoryService.Setup(s => s.GetBookStockAsync(1, 9999)).ReturnsAsync((BranchBookStockDto?)null);

        var result = await _controller.GetBookStock(1, 9999);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetBookStock_Found_ReturnsOk()
    {
        var stock = MakeStock();
        _inventoryService.Setup(s => s.GetBookStockAsync(1, 1001)).ReturnsAsync(stock);

        var result = await _controller.GetBookStock(1, 1001);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(stock, ok.Value);
    }

    // ── UpdateInventory ───────────────────────────────────────────────────────

    [TestMethod]
    public async Task UpdateInventory_ZeroBranchId_ReturnsBadRequest()
    {
        var result = await _controller.UpdateInventory(new UpdateInventoryDto(0, 1001, 5));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateInventory_ZeroBookISBN_ReturnsBadRequest()
    {
        var result = await _controller.UpdateInventory(new UpdateInventoryDto(1, 0, 5));
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateInventory_ServiceReturnsFalse_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.UpdateInventoryAsync(It.IsAny<UpdateInventoryDto>())).ReturnsAsync(false);

        var result = await _controller.UpdateInventory(new UpdateInventoryDto(1, 1001, 5));

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateInventory_Success_ReturnsOk()
    {
        _inventoryService.Setup(s => s.UpdateInventoryAsync(It.IsAny<UpdateInventoryDto>())).ReturnsAsync(true);

        var result = await _controller.UpdateInventory(new UpdateInventoryDto(1, 1001, 5));

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── AddBooks ──────────────────────────────────────────────────────────────

    [TestMethod]
    public async Task AddBooks_ZeroParameters_ReturnsBadRequest()
    {
        var result = await _controller.AddBooks(0, 1001, 5);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task AddBooks_ZeroCount_ReturnsBadRequest()
    {
        var result = await _controller.AddBooks(1, 1001, 0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task AddBooks_ServiceReturnsFalse_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.AddBooksAsync(1, 1001, 5)).ReturnsAsync(false);

        var result = await _controller.AddBooks(1, 1001, 5);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task AddBooks_Success_ReturnsOk()
    {
        _inventoryService.Setup(s => s.AddBooksAsync(1, 1001, 5)).ReturnsAsync(true);

        var result = await _controller.AddBooks(1, 1001, 5);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── RemoveBooks ───────────────────────────────────────────────────────────

    [TestMethod]
    public async Task RemoveBooks_ZeroParameters_ReturnsBadRequest()
    {
        var result = await _controller.RemoveBooks(1, 0, 3);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task RemoveBooks_ZeroCount_ReturnsBadRequest()
    {
        var result = await _controller.RemoveBooks(1, 1001, 0);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task RemoveBooks_ServiceReturnsFalse_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.RemoveBooksAsync(1, 1001, 3)).ReturnsAsync(false);

        var result = await _controller.RemoveBooks(1, 1001, 3);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task RemoveBooks_Success_ReturnsOk()
    {
        _inventoryService.Setup(s => s.RemoveBooksAsync(1, 1001, 3)).ReturnsAsync(true);

        var result = await _controller.RemoveBooks(1, 1001, 3);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        Assert.AreEqual(true, ok.Value);
    }

    // ── GetAllInventory ───────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetAllInventory_ReturnsOkWithItems()
    {
        var items = new List<BranchBookStockDto> { MakeStock(1001), MakeStock(1002) };
        _inventoryService.Setup(s => s.GetAllBranchBookInventoriesAsync()).ReturnsAsync(items);

        var result = await _controller.GetAllInventory();

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetAllInventory_ServiceThrows_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.GetAllBranchBookInventoriesAsync()).ThrowsAsync(new Exception("error"));

        var result = await _controller.GetAllInventory();

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    // ── GetLowStockItems ──────────────────────────────────────────────────────

    [TestMethod]
    public async Task GetLowStockItems_ReturnsOkWithLowStockItems()
    {
        var items = new List<BranchBookStockDto> { MakeStock(1001, 2) };
        _inventoryService.Setup(s => s.GetLowStockItemsAsync(5)).ReturnsAsync(items);

        var result = await _controller.GetLowStockItems(5);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetLowStockItems_NegativeThreshold_UsesDefaultFive()
    {
        var items = new List<BranchBookStockDto>();
        _inventoryService.Setup(s => s.GetLowStockItemsAsync(5)).ReturnsAsync(items);

        // negative threshold gets corrected to 5 in the controller
        var result = await _controller.GetLowStockItems(-1);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        _inventoryService.Verify(s => s.GetLowStockItemsAsync(5), Times.Once);
    }

    [TestMethod]
    public async Task GetLowStockItems_ServiceThrows_ReturnsBadRequest()
    {
        _inventoryService.Setup(s => s.GetLowStockItemsAsync(It.IsAny<int>())).ThrowsAsync(new Exception("error"));

        var result = await _controller.GetLowStockItems(5);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
}
