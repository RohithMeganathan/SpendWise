using Microsoft.EntityFrameworkCore;
using IncomeExpenseManagementApp.Models;

namespace IncomeExpenseManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map table names to lowercase (PostgreSQL case-sensitivity)
            modelBuilder.Entity<Transaction>().ToTable("transactions");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<TransactionType>().ToTable("transactiontype");

            // Seed initial transaction types
            modelBuilder.Entity<TransactionType>().HasData(
                new TransactionType { Id = 1, Name = "Income" },
                new TransactionType { Id = 2, Name = "Expense" }
            );

            // Configure Transaction table
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure relationships
            modelBuilder.Entity<Category>()
                .HasOne(c => c.TransactionType)
                .WithMany(tt => tt.Categories)
                .HasForeignKey(c => c.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
