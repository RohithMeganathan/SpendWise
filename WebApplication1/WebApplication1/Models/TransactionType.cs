using System.ComponentModel.DataAnnotations;

namespace IncomeExpenseManagementApp.Models
{
    public class TransactionType
    {
        [Key]
        public byte Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
