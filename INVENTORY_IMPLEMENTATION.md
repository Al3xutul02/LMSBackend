# Inventory Management Implementation

## Overview
Complete inventory management system has been implemented for the LMS Backend. This enables the "Manage Library Inventory" feature to display and manage book inventory across library branches.

## What Was Created

### 1. DTOs (Data Transfer Objects)
**Location:** `BusinessLogic/DTOs/Inventory/`

- **InventoryStatsDto** - Overall inventory statistics across all branches
  - TotalBooks: Total number of books in inventory
  - AvailableBooks: Books available for borrowing
  - BorrowedBooks: Currently borrowed books
  - TotalBranches: Number of branches
  - BranchInventories: Detailed inventory per branch

- **BranchInventoryDto** - Inventory details for a specific branch
  - BranchId, BranchName
  - TotalBooks, UniqueBooks
  - Books: List of books at the branch with stock details

- **BranchBookStockDto** - Stock details for a specific book at a branch
  - BookISBN, BookTitle, BookAuthor
  - AvailableCount, TotalCount, BorrowedCount

- **UpdateInventoryDto** - Request to update inventory
  - BranchId, BookISBN, Count, Reason

### 2. Service Layer
**Location:** `BusinessLogic/Services/`

- **IInventoryService** (Interface) - Abstract/IInventoryService.cs
  - GetInventoryStatsAsync() - Get overall statistics
  - GetBranchInventoryAsync(branchId) - Get branch inventory
  - GetBookStockAsync(branchId, bookISBN) - Get specific book stock
  - UpdateInventoryAsync(dto) - Update inventory
  - AddBooksAsync(branchId, bookISBN, count) - Add books
  - RemoveBooksAsync(branchId, bookISBN, count) - Remove books
  - GetAllBranchBookInventoriesAsync() - Get all inventory
  - GetLowStockItemsAsync(threshold) - Get items below threshold

- **InventoryService** (Implementation) - InventoryService.cs
  - Full implementation of inventory management logic
  - Calculates available vs borrowed books
  - Manages branch-book relationships
  - Tracks low stock items

### 3. API Controller
**Location:** `LMS Backend/Controllers/`

**InventoryController.cs** - RESTful endpoints:

#### Statistics
- `GET /inventory/stats` - Get overall inventory statistics
  - Returns: InventoryStatsDto with total books, available books, borrowed books

#### Branch Operations
- `GET /inventory/branch/{branchId}` - Get branch inventory details
- `GET /inventory/branch/{branchId}/book/{bookISBN}` - Get specific book stock at branch

#### Update Operations
- `POST /inventory/update` - Update inventory (add/remove books)
  - Request body: UpdateInventoryDto
- `POST /inventory/add/{branchId}/{bookISBN}/{count}` - Add books to branch
- `POST /inventory/remove/{branchId}/{bookISBN}/{count}` - Remove books from branch

#### Reporting
- `GET /inventory/all` - Get all inventory items across all branches
- `GET /inventory/low-stock?threshold=5` - Get low stock items

### 4. Dependency Injection
**Updated:** `LMS Backend/Program.cs`

Added service registration:
```csharp
.AddScoped<IInventoryService, InventoryService>()
```

The InventoryService automatically injects:
- IBranchBookRelationRepository
- IBranchRepository
- IBookRepository
- ILoanRepository
- IMapper

## How It Works

### Data Flow
1. **Repository Layer** - Accesses BranchBookRelation table (stores book count per branch)
2. **Service Layer** - Combines data from multiple repositories:
   - BranchBookRelation (inventory counts)
   - Book (book details)
   - Branch (branch details)
   - Loan (active loans to calculate borrowed books)
3. **Controller Layer** - Exposes HTTP endpoints for the frontend

### Key Calculations
- **Total Books** = Sum of Count in BranchBookRelation
- **Borrowed Books** = Sum of counts in active loans
- **Available Books** = Total Books - Borrowed Books
- **Stock Per Branch** = BranchBookRelation records filtered by branch

## UI Integration

### For "Manage Library Inventory" Dashboard
Call the stats endpoint to populate the UI:

```
GET /inventory/stats
```

Response:
```json
{
  "totalBooks": 0,
  "availableBooks": 0,
  "borrowedBooks": 0,
  "totalBranches": 0,
  "branchInventories": [...]
}
```

### For "Update Inventory" Button
Send a POST request to update inventory:

```
POST /inventory/update
Content-Type: application/json

{
  "branchId": 1,
  "bookISBN": 1234567890,
  "count": 5,
  "reason": "New stock received"
}
```

### For "Manage Returns" Button
Use the loan endpoints to process returns, which automatically updates inventory availability.

## Testing the Implementation

### 1. Build the Solution
```
dotnet build
```

### 2. Run the Application
```
dotnet run
```

### 3. Access Swagger UI
Navigate to: `https://localhost:7xxx/` (port may vary)
Look for "inventory" controller to test all endpoints

### 4. Test Endpoints
In Swagger UI, expand the "inventory" section and test:
1. `GET /inventory/stats` - Should return current inventory stats
2. `POST /inventory/update` - Add books to inventory
3. `GET /inventory/branch/{id}` - View branch inventory

## Database Requirements

The existing database tables are used:
- **BranchBookRelation** - Stores book count per branch
- **Loan** - Used to calculate borrowed books
- **LoanBookRelation** - Details of books in each loan
- **Book** - Book information
- **Branch** - Branch information

No database schema changes were required.

## Notes
- All operations return proper HTTP status codes (200, 400, 404)
- Error handling included in all methods
- Asynchronous operations for better performance
- Uses AutoMapper for DTO conversion where needed
- CORS configured for frontend communication
