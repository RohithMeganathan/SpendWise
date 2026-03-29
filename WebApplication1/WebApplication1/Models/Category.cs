using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncomeExpenseManagementApp.Models
{
    public class Category
    {
        [Key]
        public short Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [ForeignKey("TransactionType")]
        public short TransactionTypeId { get; set; }

        // Navigation properties
        public TransactionType? TransactionType { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
