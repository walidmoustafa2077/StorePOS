using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleCart> SaleLines => Set<SaleCart>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Product>(e =>
            {
                e.HasIndex(x => x.Sku).IsUnique();
                e.HasIndex(x => x.Barcode);
                e.Property(x => x.Price).HasPrecision(18, 2);
                e.Property(x => x.Cost).HasPrecision(18, 2);
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            });

            b.Entity<Sale>(e =>
            {
                e.Property(x => x.Subtotal).HasPrecision(18, 2);
                e.Property(x => x.Discount).HasPrecision(18, 2);
                e.Property(x => x.Tax).HasPrecision(18, 2);
                e.Property(x => x.Total).HasPrecision(18, 2);
                e.Property(x => x.PaidAmount).HasPrecision(18, 2);
                e.Property(x => x.Notes).HasMaxLength(500);
                
                e.HasMany(x => x.Carts)
                 .WithOne(x => x.Sale)
                 .HasForeignKey(x => x.SaleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<SaleCart>(e =>
            {
                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.Property(x => x.LineTotal).HasPrecision(18, 2);
                
                e.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<User>(e =>
            {
                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Username).HasMaxLength(50).IsUnicode();
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();
                e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                
                e.HasMany(x => x.RefreshTokens)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<RefreshToken>(e =>
            {
                e.Property(x => x.Token).HasMaxLength(255).IsRequired();
                e.Property(x => x.CreatedByIp).HasMaxLength(45);
                e.Property(x => x.RevokedByIp).HasMaxLength(45);
                e.Property(x => x.ReplaceByToken).HasMaxLength(255);
            });
        }
    }
}
