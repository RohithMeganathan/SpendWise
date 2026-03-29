using System.ComponentModel.DataAnnotations;

namespace IncomeExpenseManagementApp.DTOs
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public byte CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public byte TransactionTypeId { get; set; }
        public string? TransactionTypeName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }

    public class CreateTransactionDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public byte CategoryId { get; set; }

        [Required]
        [Range(0.01, 999999999.99)]
        public decimal Amount { get; set; }

        public DateTime? TransactionDate { get; set; }
    }

    public class UpdateTransactionDTO
    {
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public byte? CategoryId { get; set; }

        [Range(0.01, 999999999.99)]
        public decimal? Amount { get; set; }
    }
}
