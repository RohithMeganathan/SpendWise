using System.Linq.Expressions;

namespace IncomeExpenseManagementApp.Repositories
{
    public interface IRepository<T> where T : class
    {
        // Read operations
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Write operations
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> DeleteAsync(T entity);
        Task<int> DeleteRangeAsync(Expression<Func<T, bool>> predicate);

        // Save changes
        Task<int> SaveChangesAsync();
    }
}
