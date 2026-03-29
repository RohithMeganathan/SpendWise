using IncomeExpenseManagementApp.DTOs;

namespace IncomeExpenseManagementApp.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryDTO>> GetCategoriesByTypeAsync(short transactionTypeId);
        Task<CategoryDTO?> GetCategoryByIdAsync(short id);
        Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDTO);
        Task<bool> DeleteCategoryAsync(short id);
    }

    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync(short? categoryId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<TransactionDTO?> GetTransactionByIdAsync(long id);
        Task<TransactionDTO> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO);
        Task<TransactionDTO?> UpdateTransactionAsync(long id, UpdateTransactionDTO updateTransactionDTO);
        Task<bool> DeleteTransactionAsync(long id);
    }

    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetDashboardSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<MonthlySummaryDTO>> GetMonthlySummaryAsync();
    }
}
