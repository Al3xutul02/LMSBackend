using AutoMapper;
using BusinessLogic.DTOs.Inventory;
using BusinessLogic.Services.Abstract;
using Repository.Repositories.Abstract;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;
using Repository.Tables;

namespace BusinessLogic.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IBranchBookRelationRepository _branchBookRelationRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IBranchBookRelationRepository branchBookRelationRepository,
            IBranchRepository branchRepository,
            IBookRepository bookRepository,
            ILoanRepository loanRepository,
            IMapper mapper)
        {
            _branchBookRelationRepository = branchBookRelationRepository;
            _branchRepository = branchRepository;
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        private static bool IsBookOut(LoanStatus status)
        {
            string s = status.ToString().ToLower();
            return s == "active" || s == "overdue";
        }

        public async Task<InventoryStatsDto?> GetInventoryStatsAsync()
        {
            try
            {
                // Preluăm datele
                var allInventories = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var branches = await _branchRepository.GetAllAsync(IncludeBehavior.NoInclude);

                // 1. Totalul rămâne 30
                int totalBooks = allInventories.Sum(i => i.Count);

                // 2. Numărăm câte rânduri de împrumut au statusul Active sau Overdue
                int finalBorrowedCount = loans.Count(l => IsBookOut(l.Status));
                Console.WriteLine($"[DEBUG] Total loans: {loans.Count()}, Borrowed (Active/Overdue): {finalBorrowedCount}");
                foreach (var loan in loans)
                {
                    Console.WriteLine($"[DEBUG] Loan {loan.Id}: Status={loan.Status}, IsOut={IsBookOut(loan.Status)}");
                }

                // 3. Calculul final: Total - Borrowed
                int availableBooks = totalBooks - finalBorrowedCount;

                var branchInventories = await GetAllBranchInventoriesDetailedAsync();

                return new InventoryStatsDto(
                    TotalBooks: totalBooks,
                    AvailableBooks: availableBooks,
                    BorrowedBooks: finalBorrowedCount, 
                    TotalBranches: branches.Count(),
                    BranchInventories: branchInventories
                );
            }
            catch
            {
                return null;
            }
        }

        public async Task<BranchInventoryDto?> GetBranchInventoryAsync(int branchId)
        {
            try
            {
                var branch = await _branchRepository.GetByIdAsync(branchId, IncludeBehavior.NoInclude);
                if (branch == null) return null;

                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var branchInventory = inventory.Where(i => i.BranchId == branchId).ToList();

                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var outstandingLoans = loans.Where(l => IsBookOut(l.Status)).ToList();

                var books = new List<BranchBookStockDto>();
                int totalBooks = 0;

                foreach (var item in branchInventory)
                {
                    if (item.Book != null)
                    {
                        // Aplicăm aceeași logică de siguranță și aici
                        int relationSum = outstandingLoans
                            .SelectMany(l => l.Books ?? new List<LoanBookRelation>())
                            .Where(r => r.BookISBN == item.BookISBN)
                            .Sum(r => r.Count);

                        // Dacă suma relațiilor e 0 dar cartea apare în împrumuturi active, punem minim 1
                        int loanEntityCount = outstandingLoans.Count(l => l.Books != null && l.Books.Any(r => r.BookISBN == item.BookISBN));
                        int finalBorrowed = Math.Max(relationSum, loanEntityCount);

                        int availableCount = item.Count - finalBorrowed;

                        books.Add(new BranchBookStockDto(
                            BookISBN: item.BookISBN,
                            BookTitle: item.Book.Title,
                            BookAuthor: item.Book.Author,
                            AvailableCount: Math.Max(0, availableCount),
                            TotalCount: item.Count,
                            BorrowedCount: finalBorrowed
                        ));

                        totalBooks += item.Count;
                    }
                }

                return new BranchInventoryDto(
                    BranchId: branchId,
                    BranchName: branch.Name,
                    TotalBooks: totalBooks,
                    UniqueBooks: books.Count,
                    Books: books
                );
            }
            catch
            {
                return null;
            }
        }

        public async Task<BranchBookStockDto?> GetBookStockAsync(int branchId, int bookISBN)
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var item = inventory.FirstOrDefault(i => i.BranchId == branchId && i.BookISBN == bookISBN);

                if (item == null || item.Book == null) return null;

                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var activeLoans = loans.Where(l => IsBookOut(l.Status)).ToList();

                int relationSum = activeLoans
                    .SelectMany(l => l.Books ?? new List<LoanBookRelation>())
                    .Where(r => r.BookISBN == bookISBN)
                    .Sum(r => r.Count);

                int loanEntityCount = activeLoans.Count(l => l.Books != null && l.Books.Any(r => r.BookISBN == bookISBN));
                int borrowedCount = Math.Max(relationSum, loanEntityCount);

                return new BranchBookStockDto(
                    BookISBN: item.BookISBN,
                    BookTitle: item.Book.Title,
                    BookAuthor: item.Book.Author,
                    AvailableCount: Math.Max(0, item.Count - borrowedCount),
                    TotalCount: item.Count,
                    BorrowedCount: borrowedCount
                );
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<BranchBookStockDto>> GetAllBranchBookInventoriesAsync()
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var activeLoans = loans.Where(l => IsBookOut(l.Status)).ToList();

                var result = new List<BranchBookStockDto>();

                foreach (var item in inventory)
                {
                    if (item.Book != null)
                    {
                        int relationSum = activeLoans
                            .SelectMany(l => l.Books ?? new List<LoanBookRelation>())
                            .Where(r => r.BookISBN == item.BookISBN)
                            .Sum(r => r.Count);

                        int loanEntityCount = activeLoans.Count(l => l.Books != null && l.Books.Any(r => r.BookISBN == item.BookISBN));
                        int borrowedCount = Math.Max(relationSum, loanEntityCount);

                        result.Add(new BranchBookStockDto(
                            BookISBN: item.BookISBN,
                            BookTitle: item.Book.Title,
                            BookAuthor: item.Book.Author,
                            AvailableCount: Math.Max(0, item.Count - borrowedCount),
                            TotalCount: item.Count,
                            BorrowedCount: borrowedCount
                        ));
                    }
                }

                return result;
            }
            catch
            {
                return new List<BranchBookStockDto>();
            }
        }

        public async Task<bool> UpdateInventoryAsync(UpdateInventoryDto dto)
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.NoInclude);
                var item = inventory.FirstOrDefault(i => i.BranchId == dto.BranchId && i.BookISBN == dto.BookISBN);

                if (item == null)
                {
                    if (dto.Count > 0)
                    {
                        var newItem = new BranchBookRelation
                        {
                            BranchId = dto.BranchId,
                            BookISBN = dto.BookISBN,
                            Count = dto.Count
                        };
                        await _branchBookRelationRepository.AddAsync(newItem);
                    }
                }
                else
                {
                    item.Count = Math.Max(0, item.Count + dto.Count);
                    _branchBookRelationRepository.Update(item);
                }

                await _branchBookRelationRepository.SaveAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> AddBooksAsync(int branchId, int bookISBN, int count) =>
            count > 0 && await UpdateInventoryAsync(new UpdateInventoryDto(branchId, bookISBN, count, "Added"));

        public async Task<bool> RemoveBooksAsync(int branchId, int bookISBN, int count) =>
            count > 0 && await UpdateInventoryAsync(new UpdateInventoryDto(branchId, bookISBN, -count, "Removed"));

        public async Task<IEnumerable<BranchBookStockDto>> GetLowStockItemsAsync(int threshold = 5)
        {
            var allItems = await GetAllBranchBookInventoriesAsync();
            return allItems.Where(i => i.AvailableCount < threshold);
        }

        private async Task<IEnumerable<BranchInventoryDto>> GetAllBranchInventoriesDetailedAsync()
        {
            var branches = await _branchRepository.GetAllAsync(IncludeBehavior.NoInclude);
            var result = new List<BranchInventoryDto>();
            foreach (var branch in branches)
            {
                var branchInventory = await GetBranchInventoryAsync(branch.Id);
                if (branchInventory != null) result.Add(branchInventory);
            }
            return result;
        }
    }
}