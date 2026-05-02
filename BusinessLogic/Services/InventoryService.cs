using AutoMapper;
using BusinessLogic.DTOs.Inventory;
using BusinessLogic.Services.Abstract;
using Repository.Repositories.Abstract;
using Repository.Enums.Behaviors;
using Repository.Enums.Types;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Service for managing library inventory across branches.
    /// </summary>
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

        /// <summary>
        /// Gets inventory statistics across all branches.
        /// </summary>
        public async Task<InventoryStatsDto?> GetInventoryStatsAsync()
        {
            try
            {
                var allInventories = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var branches = await _branchRepository.GetAllAsync(IncludeBehavior.NoInclude);

                int totalBooks = allInventories.Sum(i => i.Count);
                int borrowedBooks = loans
                    .Where(l => l.Status == LoanStatus.Active)
                    .SelectMany(l => l.Books ?? new List<Repository.Tables.LoanBookRelation>())
                    .Sum(r => r.Count);

                int availableBooks = totalBooks - borrowedBooks;

                var branchInventories = await GetAllBranchInventoriesDetailedAsync();

                return new InventoryStatsDto(
                    TotalBooks: totalBooks,
                    AvailableBooks: availableBooks,
                    BorrowedBooks: borrowedBooks,
                    TotalBranches: branches.Count(),
                    BranchInventories: branchInventories
                );
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets inventory details for a specific branch.
        /// </summary>
        public async Task<BranchInventoryDto?> GetBranchInventoryAsync(int branchId)
        {
            try
            {
                var branch = await _branchRepository.GetByIdAsync(branchId, IncludeBehavior.NoInclude);
                if (branch == null) return null;

                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var branchInventory = inventory.Where(i => i.BranchId == branchId).ToList();

                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var activeLoans = loans.Where(l => l.Status == LoanStatus.Active).ToList();

                var books = new List<BranchBookStockDto>();
                int totalBooks = 0;

                foreach (var item in branchInventory)
                {
                    if (item.Book != null)
                    {
                        int borrowedCount = activeLoans
                            .SelectMany(l => l.Books ?? new List<Repository.Tables.LoanBookRelation>())
                            .Where(r => r.BookISBN == item.BookISBN)
                            .Sum(r => r.Count);

                        int availableCount = item.Count - borrowedCount;

                        books.Add(new BranchBookStockDto(
                            BookISBN: item.BookISBN,
                            BookTitle: item.Book.Title,
                            BookAuthor: item.Book.Author,
                            AvailableCount: Math.Max(0, availableCount),
                            TotalCount: item.Count,
                            BorrowedCount: borrowedCount
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

        /// <summary>
        /// Gets stock details for a specific book at a specific branch.
        /// </summary>
        public async Task<BranchBookStockDto?> GetBookStockAsync(int branchId, int bookISBN)
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var item = inventory.FirstOrDefault(i => i.BranchId == branchId && i.BookISBN == bookISBN);

                if (item == null || item.Book == null) return null;

                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var borrowedCount = loans
                    .Where(l => l.Status == LoanStatus.Active)
                    .SelectMany(l => l.Books ?? new List<Repository.Tables.LoanBookRelation>())
                    .Where(r => r.BookISBN == bookISBN)
                    .Sum(r => r.Count);

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

        /// <summary>
        /// Updates inventory (add or remove books from branch).
        /// </summary>
        public async Task<bool> UpdateInventoryAsync(UpdateInventoryDto dto)
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.NoInclude);
                var item = inventory.FirstOrDefault(i => i.BranchId == dto.BranchId && i.BookISBN == dto.BookISBN);

                if (item == null)
                {
                    // Create new inventory record if it doesn't exist
                    if (dto.Count > 0)
                    {
                        var newItem = new Repository.Tables.BranchBookRelation
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
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds books to a branch inventory.
        /// </summary>
        public async Task<bool> AddBooksAsync(int branchId, int bookISBN, int count)
        {
            if (count <= 0) return false;
            return await UpdateInventoryAsync(new UpdateInventoryDto(branchId, bookISBN, count, "Added"));
        }

        /// <summary>
        /// Removes books from a branch inventory.
        /// </summary>
        public async Task<bool> RemoveBooksAsync(int branchId, int bookISBN, int count)
        {
            if (count <= 0) return false;
            return await UpdateInventoryAsync(new UpdateInventoryDto(branchId, bookISBN, -count, "Removed"));
        }

        /// <summary>
        /// Gets all books across all branches (inventory view).
        /// </summary>
        public async Task<IEnumerable<BranchBookStockDto>> GetAllBranchBookInventoriesAsync()
        {
            try
            {
                var inventory = await _branchBookRelationRepository.GetAllAsync(IncludeBehavior.AllIncludes);
                var loans = await _loanRepository.GetAllAsync(IncludeBehavior.AllIncludes);

                var result = new List<BranchBookStockDto>();

                foreach (var item in inventory)
                {
                    if (item.Book != null)
                    {
                        int borrowedCount = loans
                            .Where(l => l.Status == LoanStatus.Active)
                            .SelectMany(l => l.Books ?? new List<Repository.Tables.LoanBookRelation>())
                            .Where(r => r.BookISBN == item.BookISBN)
                            .Sum(r => r.Count);

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

        /// <summary>
        /// Gets low stock items (books with count below threshold).
        /// </summary>
        public async Task<IEnumerable<BranchBookStockDto>> GetLowStockItemsAsync(int threshold = 5)
        {
            try
            {
                var allItems = await GetAllBranchBookInventoriesAsync();
                return allItems.Where(i => i.AvailableCount < threshold);
            }
            catch
            {
                return new List<BranchBookStockDto>();
            }
        }

        private async Task<IEnumerable<BranchInventoryDto>> GetAllBranchInventoriesDetailedAsync()
        {
            var branches = await _branchRepository.GetAllAsync(IncludeBehavior.NoInclude);
            var result = new List<BranchInventoryDto>();

            foreach (var branch in branches)
            {
                var branchInventory = await GetBranchInventoryAsync(branch.Id);
                if (branchInventory != null)
                {
                    result.Add(branchInventory);
                }
            }

            return result;
        }
    }
}
