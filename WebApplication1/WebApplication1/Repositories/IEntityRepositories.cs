using IncomeExpenseManagementApp.Models;

namespace IncomeExpenseManagementApp.Repositories
{
    public interface ITransactionTypeRepository : IRepository<TransactionType>
    {
        Task<TransactionType?> GetByNameAsync(string name);
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByTransactionTypeAsync(byte transactionTypeId);
        Task<Category?> GetWithTransactionTypeAsync(byte id);
    }

    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetWithCategoryAsync();
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetByCategoryAsync(byte categoryId);
        Task<Transaction?> GetWithCategoryByCategoryAsync(byte categoryId);
    }
}
