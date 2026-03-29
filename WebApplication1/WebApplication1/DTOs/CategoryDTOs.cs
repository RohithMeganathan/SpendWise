using System.ComponentModel.DataAnnotations;

namespace IncomeExpenseManagementApp.DTOs
{
    public class CategoryDTO
    {
        public byte Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte TransactionTypeId { get; set; }
        public string? TransactionTypeName { get; set; }
    }

    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public byte TransactionTypeId { get; set; }
    }
}
