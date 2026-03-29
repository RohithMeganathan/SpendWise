using Microsoft.EntityFrameworkCore;
using IncomeExpenseManagementApp.Data;
using IncomeExpenseManagementApp.Models;

namespace IncomeExpenseManagementApp.Repositories
{
    public class TransactionTypeRepository : Repository<TransactionType>, ITransactionTypeRepository
    {
        public TransactionTypeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<TransactionType?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(tt => tt.Name == name);
        }
    }

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetByTransactionTypeAsync(byte transactionTypeId)
        {
            return await _dbSet
                .Where(c => c.TransactionTypeId == transactionTypeId)
                .Include(c => c.TransactionType)
                .ToListAsync();
        }

        public async Task<Category?> GetWithTransactionTypeAsync(byte id)
        {
            return await _dbSet
                .Include(c => c.TransactionType)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetWithCategoryAsync()
        {
            return await _dbSet
                .Include(t => t.Category)
                .ThenInclude(c => c.TransactionType)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(t => t.Category)
                .ThenInclude(c => c.TransactionType)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate.AddDays(1))
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByCategoryAsync(byte categoryId)
        {
            return await _dbSet
                .Include(t => t.Category)
                .ThenInclude(c => c.TransactionType)
                .Where(t => t.CategoryId == categoryId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetWithCategoryByCategoryAsync(byte categoryId)
        {
            return await _dbSet
                .Include(t => t.Category)
                .ThenInclude(c => c.TransactionType)
                .FirstOrDefaultAsync(t => t.CategoryId == categoryId);
        }
    }
}
