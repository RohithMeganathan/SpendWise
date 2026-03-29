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

            // Configure Transaction table and column mappings
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id).HasColumnName("id");
                entity.Property(t => t.Name).HasColumnName("name");
                entity.Property(t => t.Description).HasColumnName("description");
                entity.Property(t => t.CategoryId).HasColumnName("categoryid");
                entity.Property(t => t.Amount).HasColumnName("amount");
                entity.Property(t => t.TransactionDate).HasColumnName("transactiondate").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure Category table and column mappings
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasColumnName("id");
                entity.Property(c => c.Name).HasColumnName("name");
                entity.Property(c => c.TransactionTypeId).HasColumnName("transactiontypeid");
            });

            // Configure TransactionType table and column mappings
            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.HasKey(tt => tt.Id);
                entity.Property(tt => tt.Id).HasColumnName("id");
                entity.Property(tt => tt.Name).HasColumnName("name");
            });

            // Seed initial transaction types
            modelBuilder.Entity<TransactionType>().HasData(
                new TransactionType { Id = 1, Name = "Income" },
                new TransactionType { Id = 2, Name = "Expense" }
            );

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
