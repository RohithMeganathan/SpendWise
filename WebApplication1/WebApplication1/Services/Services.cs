using IncomeExpenseManagementApp.DTOs;
using IncomeExpenseManagementApp.Models;
using IncomeExpenseManagementApp.Repositories;

namespace IncomeExpenseManagementApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITransactionTypeRepository _transactionTypeRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ITransactionTypeRepository transactionTypeRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _transactionTypeRepository = transactionTypeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Select(MapToDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all categories");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByTypeAsync(byte transactionTypeId)
        {
            try
            {
                var categories = await _categoryRepository.GetByTransactionTypeAsync(transactionTypeId);
                return categories.Select(MapToDTO).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories by type");
                throw;
            }
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(byte id)
        {
            try
            {
                var category = await _categoryRepository.GetWithTransactionTypeAsync(id);
                return category != null ? MapToDTO(category) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category by ID");
                throw;
            }
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDTO)
        {
            try
            {
                // Validate transaction type exists
                var transactionTypeExists = await _transactionTypeRepository
                    .AnyAsync(tt => tt.Id == createCategoryDTO.TransactionTypeId);

                if (!transactionTypeExists)
                {
                    throw new ArgumentException("Invalid transaction type ID");
                }

                var category = new Category
                {
                    Name = createCategoryDTO.Name,
                    TransactionTypeId = createCategoryDTO.TransactionTypeId
                };

                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                _logger.LogInformation("Category created with ID: {CategoryId}", category.Id);

                return MapToDTO(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(byte id)
        {
            try
            {
                var result = await _categoryRepository.DeleteAsync(id);
                if (result)
                {
                    await _categoryRepository.SaveChangesAsync();
                    _logger.LogInformation("Category deleted with ID: {CategoryId}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                throw;
            }
        }

        private CategoryDTO MapToDTO(Category category)
        {
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                TransactionTypeId = category.TransactionTypeId,
                TransactionTypeName = category.TransactionType?.Name
            };
        }
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ICategoryRepository categoryRepository,
            ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync(
            byte? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                var transactions = await _transactionRepository.GetWithCategoryAsync();

                if (categoryId.HasValue)
                {
                    transactions = transactions.Where(t => t.CategoryId == categoryId.Value).ToList();
                }

                if (startDate.HasValue)
                {
                    transactions = transactions.Where(t => t.TransactionDate >= startDate.Value).ToList();
                }

                if (endDate.HasValue)
                {
                    transactions = transactions.Where(t => t.TransactionDate <= endDate.Value.AddDays(1)).ToList();
                }

                return transactions.Select(MapToDTO).OrderByDescending(t => t.TransactionDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                throw;
            }
        }

        public async Task<TransactionDTO?> GetTransactionByIdAsync(long id)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(id);
                if (transaction == null) return null;

                // Ensure category and transaction type are loaded
                if (transaction.Category == null)
                {
                    var category = await _categoryRepository.GetWithTransactionTypeAsync(transaction.CategoryId);
                    transaction.Category = category;
                }
                else if (transaction.Category.TransactionType == null)
                {
                    var category = await _categoryRepository.GetWithTransactionTypeAsync(transaction.CategoryId);
                    transaction.Category.TransactionType = category?.TransactionType;
                }

                return MapToDTO(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction by ID");
                throw;
            }
        }

        public async Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO)
        {
            try
            {
                // Validate category exists
                var category = await _categoryRepository.GetWithTransactionTypeAsync(createTransactionDTO.CategoryId);
                if (category == null)
                {
                    throw new ArgumentException("Invalid category ID");
                }

                var transaction = new Transaction
                {
                    Name = createTransactionDTO.Name,
                    Description = createTransactionDTO.Description,
                    CategoryId = createTransactionDTO.CategoryId,
                    Amount = createTransactionDTO.Amount,
                    TransactionDate = createTransactionDTO.TransactionDate ?? DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Transaction created with ID: {TransactionId}", transaction.Id);

                transaction.Category = category;
                return MapToDTO(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                throw;
            }
        }

        public async Task<TransactionDTO?> UpdateTransactionAsync(long id, UpdateTransactionDTO updateTransactionDTO)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(id);
                if (transaction == null) return null;

                if (updateTransactionDTO.CategoryId.HasValue)
                {
                    var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == updateTransactionDTO.CategoryId.Value);
                    if (!categoryExists)
                    {
                        throw new ArgumentException("Invalid category ID");
                    }
                    transaction.CategoryId = updateTransactionDTO.CategoryId.Value;
                }

                if (!string.IsNullOrEmpty(updateTransactionDTO.Name))
                    transaction.Name = updateTransactionDTO.Name;

                if (!string.IsNullOrEmpty(updateTransactionDTO.Description))
                    transaction.Description = updateTransactionDTO.Description;

                if (updateTransactionDTO.Amount.HasValue)
                    transaction.Amount = updateTransactionDTO.Amount.Value;

                await _transactionRepository.UpdateAsync(transaction);
                await _transactionRepository.SaveChangesAsync();

                _logger.LogInformation("Transaction updated with ID: {TransactionId}", id);

                // Load related data
                var category = await _categoryRepository.GetWithTransactionTypeAsync(transaction.CategoryId);
                transaction.Category = category;

                return MapToDTO(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction");
                throw;
            }
        }

        public async Task<bool> DeleteTransactionAsync(long id)
        {
            try
            {
                var result = await _transactionRepository.DeleteAsync(id);
                if (result)
                {
                    await _transactionRepository.SaveChangesAsync();
                    _logger.LogInformation("Transaction deleted with ID: {TransactionId}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction");
                throw;
            }
        }

        private TransactionDTO MapToDTO(Transaction transaction)
        {
            return new TransactionDTO
            {
                Id = transaction.Id,
                Name = transaction.Name,
                Description = transaction.Description,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.Name,
                TransactionTypeId = transaction.Category?.TransactionTypeId ?? 0,
                TransactionTypeName = transaction.Category?.TransactionType?.Name,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate
            };
        }
    }

    public class DashboardService : IDashboardService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            ITransactionRepository transactionRepository,
            ILogger<DashboardService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<DashboardSummaryDTO> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;

                var transactions = await _transactionRepository.GetByDateRangeAsync(start, end);

                var incomeTransactions = transactions.Where(t => t.Category?.TransactionType?.Id == 1).ToList();
                var expenseTransactions = transactions.Where(t => t.Category?.TransactionType?.Id == 2).ToList();

                var totalIncome = incomeTransactions.Sum(t => t.Amount);
                var totalExpense = expenseTransactions.Sum(t => t.Amount);
                var balance = totalIncome - totalExpense;

                var incomeByCategory = incomeTransactions
                    .GroupBy(t => new { t.Category?.Id, t.Category?.Name })
                    .Select(g => new CategorySummaryDTO
                    {
                        CategoryId = g.Key.Id ?? 0,
                        CategoryName = g.Key.Name ?? string.Empty,
                        Total = g.Sum(t => t.Amount),
                        Count = g.Count()
                    })
                    .ToList();

                var expenseByCategory = expenseTransactions
                    .GroupBy(t => new { t.Category?.Id, t.Category?.Name })
                    .Select(g => new CategorySummaryDTO
                    {
                        CategoryId = g.Key.Id ?? 0,
                        CategoryName = g.Key.Name ?? string.Empty,
                        Total = g.Sum(t => t.Amount),
                        Count = g.Count()
                    })
                    .ToList();

                return new DashboardSummaryDTO
                {
                    StartDate = start,
                    EndDate = end,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = balance,
                    TransactionCount = transactions.Count(),
                    IncomeCount = incomeTransactions.Count(),
                    ExpenseCount = expenseTransactions.Count(),
                    IncomeByCategory = incomeByCategory,
                    ExpenseByCategory = expenseByCategory
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                throw;
            }
        }

        public async Task<IEnumerable<MonthlySummaryDTO>> GetMonthlySummaryAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var startDate = now.AddMonths(-12);

                var transactions = await _transactionRepository.GetByDateRangeAsync(startDate, now);

                return transactions
                    .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .Select(g => new MonthlySummaryDTO
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Income = g.Where(t => t.Category?.TransactionType?.Id == 1).Sum(t => t.Amount),
                        Expense = g.Where(t => t.Category?.TransactionType?.Id == 2).Sum(t => t.Amount),
                        TransactionCount = g.Count()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly summary");
                throw;
            }
        }
    }
}
