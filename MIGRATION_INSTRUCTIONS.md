# Database Migration Instructions

## What Changed
- Added `ReturnDate` property to the `Loan` model
- Updated `DatabaseContext.cs` to configure the new property
- Created a new endpoint for marking loans as returned

## How to Run the Migration

### Option 1: Using Package Manager Console (Easiest)
1. Open Visual Studio
2. Go to **View** → **Other Windows** → **Package Manager Console**
3. Make sure **Default project** is set to the project containing your DbContext (Repository)
4. Run these commands:

```powershell
Add-Migration AddReturnDateToLoans
Update-Database
```

### Option 2: Using .NET CLI
Open your terminal/command prompt in the project root and run:

```bash
dotnet ef migrations add AddReturnDateToLoans -p Repository
dotnet ef database update -p Repository
```

### Option 3: Manual SQL (If you need to)
```sql
ALTER TABLE Loans
ADD ReturnDate DATETIME NULL DEFAULT NULL;
```

---

## After Migration

1. Rebuild your backend solution
2. Test the endpoints:
   - The `/Loan/get-all` endpoint should work again
   - The "Overdue Users" page should load without errors
   - The "Mark Returned" button should function properly

---

## What This Adds to the Database

- New nullable column `ReturnDate` in the `Loans` table
- When a book is marked as returned, this column is set to the current UTC time
- Helps track when books were actually returned vs. when they were due

---

## Troubleshooting

**Error: "No DbContext was found"**
- Make sure you're running the command from the project root
- Or specify the project with `-p Repository` in the .NET CLI command

**Error: "The DbContext 'DatabaseContext' cannot be used because the model has not been built"**
- This usually means the migration is trying to run before the context is fully initialized
- Try rebuilding the solution first

**Error: "Migration already exists"**
- You can check existing migrations in the `Repository/Migrations` folder
- If the migration exists, just run `Update-Database`
