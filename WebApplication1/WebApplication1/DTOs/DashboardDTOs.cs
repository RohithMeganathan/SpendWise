namespace IncomeExpenseManagementApp.DTOs
{
    public class DashboardSummaryDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public int TransactionCount { get; set; }
        public int IncomeCount { get; set; }
        public int ExpenseCount { get; set; }
        public List<CategorySummaryDTO> IncomeByCategory { get; set; } = new();
        public List<CategorySummaryDTO> ExpenseByCategory { get; set; } = new();
    }

    public class CategorySummaryDTO
    {
        public byte CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }

    public class MonthlySummaryDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public int TransactionCount { get; set; }
    }
}
