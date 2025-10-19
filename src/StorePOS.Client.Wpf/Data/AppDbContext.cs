using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;
using System.IO;

namespace StorePOS.Client.Wpf.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for SQLite database
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<SaleEntity> Sales { get; set; }
        public DbSet<SaleItemEntity> SaleItems { get; set; }
        public DbSet<ShiftEntity> Shifts { get; set; }
        public DbSet<WalletEntity> Wallets { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Get the app data directory
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dbDirectory = Path.Combine(appDataPath, "StorePOS");
                
                // Ensure directory exists
                Directory.CreateDirectory(dbDirectory);
                
                var dbPath = Path.Combine(dbDirectory, "storepos.db");
                
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ProductEntity
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.Property(e => e.Barcode).HasMaxLength(50);
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasIndex(e => e.Barcode).IsUnique();
            });

            // Configure CategoryEntity
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure SaleEntity
            modelBuilder.Entity<SaleEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.SaleNumber).IsUnique();
                entity.HasMany(e => e.Items)
                      .WithOne(e => e.Sale)
                      .HasForeignKey(e => e.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SaleItemEntity
            modelBuilder.Entity<SaleItemEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Sale)
                      .WithMany(e => e.Items)
                      .HasForeignKey(e => e.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ShiftEntity
            modelBuilder.Entity<ShiftEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShiftNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CashierName).HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.HasIndex(e => e.ShiftNumber).IsUnique();
                entity.HasMany(e => e.Sales)
                      .WithOne(e => e.Shift)
                      .HasForeignKey(e => e.ShiftId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure WalletEntity
            modelBuilder.Entity<WalletEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure TransactionEntity
            modelBuilder.Entity<TransactionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Reference).HasMaxLength(100);
                entity.Property(e => e.Timestamp).IsRequired();
                
                entity.HasOne(e => e.Wallet)
                      .WithMany()
                      .HasForeignKey(e => e.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.Shift)
                      .WithMany()
                      .HasForeignKey(e => e.ShiftId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.ShiftId);
                entity.HasIndex(e => e.WalletId);
                entity.HasIndex(e => e.Timestamp);
            });
        }
    }
}