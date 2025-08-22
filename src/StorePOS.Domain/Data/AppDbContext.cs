using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data
{
    /// <summary>
    /// Entity Framework Core database context for the StorePOS application.
    /// Manages entity relationships, database schema configuration, and data access operations.
    /// </summary>
    /// <remarks>
    /// This context implements the Code-First approach where:
    /// - Entity models define the database structure
    /// - Fluent API configures relationships and constraints
    /// - Migrations handle schema versioning and deployment
    /// </remarks>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the AppDbContext with the specified options.
        /// </summary>
        /// <param name="options">Configuration options for the database context</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the Products entity set. Represents the inventory items available for sale.
        /// </summary>
        public DbSet<Product> Products => Set<Product>();

        /// <summary>
        /// Gets or sets the Categories entity set. Represents product classification and organization.
        /// </summary>
        public DbSet<Category> Categories => Set<Category>();

        /// <summary>
        /// Gets or sets the Sales entity set. Represents completed point-of-sale transactions.
        /// </summary>
        public DbSet<Sale> Sales => Set<Sale>();

        /// <summary>
        /// Gets or sets the Sale Lines entity set. Represents individual items within a sale transaction.
        /// </summary>
        public DbSet<SaleCart> SaleLines => Set<SaleCart>();

        /// <summary>
        /// Gets or sets the Users entity set. Represents system users with authentication and authorization data.
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// Gets or sets the Refresh Tokens entity set. Manages JWT refresh tokens for secure authentication.
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        /// <summary>
        /// Configures entity relationships, constraints, and database schema using Fluent API.
        /// </summary>
        /// <param name="b">The model builder used to configure entities</param>
        /// <remarks>
        /// This method defines:
        /// - Primary and foreign key relationships
        /// - Unique constraints and indexes for performance
        /// - Decimal precision for monetary fields
        /// - String length constraints for data integrity
        /// - Cascade and restrict delete behaviors
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder b)
        {
            // Product entity configuration - Core inventory management
            b.Entity<Product>(e =>
            {
                // SKU must be unique across all products for inventory tracking
                e.HasIndex(x => x.Sku).IsUnique();
                
                // Barcode index for fast POS scanning lookups (non-unique to support multiple barcodes)
                e.HasIndex(x => x.Barcode);
                
                // Monetary fields with appropriate precision for financial calculations
                e.Property(x => x.Price).HasPrecision(18, 2);
                e.Property(x => x.Cost).HasPrecision(18, 2);
                
                // Product name is required with reasonable length limit
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            });

            // Sale entity configuration - Transaction management
            b.Entity<Sale>(e =>
            {
                // All monetary fields configured with consistent precision
                e.Property(x => x.Subtotal).HasPrecision(18, 2);
                e.Property(x => x.Discount).HasPrecision(18, 2);
                e.Property(x => x.Tax).HasPrecision(18, 2);
                e.Property(x => x.Total).HasPrecision(18, 2);
                e.Property(x => x.PaidAmount).HasPrecision(18, 2);
                
                // Optional notes field with reasonable length limit
                e.Property(x => x.Notes).HasMaxLength(500);
                
                // One-to-many relationship: Sale -> SaleCart items
                // Cascade delete ensures cart items are removed when sale is deleted
                e.HasMany(x => x.Carts)
                 .WithOne(x => x.Sale)
                 .HasForeignKey(x => x.SaleId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // SaleCart entity configuration - Individual sale line items
            b.Entity<SaleCart>(e =>
            {
                // Monetary fields for line item calculations
                e.Property(x => x.UnitPrice).HasPrecision(18, 2);
                e.Property(x => x.LineTotal).HasPrecision(18, 2);
                
                // Foreign key to Product with restrict delete behavior
                // Prevents product deletion if it's referenced in historical sales
                e.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // User entity configuration - Authentication and authorization
            b.Entity<User>(e =>
            {
                // Unique constraints for login credentials
                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
                
                // String length constraints for user data
                e.Property(x => x.Username).HasMaxLength(50).IsUnicode();
                e.Property(x => x.Email).HasMaxLength(100).IsRequired();
                e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                
                // One-to-many relationship: User -> RefreshTokens
                // Cascade delete ensures tokens are removed when user is deleted
                e.HasMany(x => x.RefreshTokens)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // RefreshToken entity configuration - JWT token management
            b.Entity<RefreshToken>(e =>
            {
                // Token storage with appropriate length for base64-encoded values
                e.Property(x => x.Token).HasMaxLength(255).IsRequired();
                
                // IP address tracking for audit trail (supports both IPv4 and IPv6)
                e.Property(x => x.CreatedByIp).HasMaxLength(45);
                e.Property(x => x.RevokedByIp).HasMaxLength(45);
                
                // Token replacement tracking for rotation pattern
                e.Property(x => x.ReplaceByToken).HasMaxLength(255);
            });
        }
    }
}
