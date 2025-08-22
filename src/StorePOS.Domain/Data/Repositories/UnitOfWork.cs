using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Unit of Work implementation that coordinates repository operations and manages database transactions.
    /// Implements the Unit of Work pattern to ensure consistency across multiple repository operations
    /// and provides a single point of transaction control.
    /// </summary>
    /// <remarks>
    /// The Unit of Work pattern provides several key benefits:
    /// - Transactional consistency across multiple repository operations
    /// - Single point of commit/rollback for complex business operations
    /// - Reduced database round trips through batched operations
    /// - Proper resource management and disposal
    /// - Centralized database context lifecycle management
    /// 
    /// This implementation manages all domain repositories and provides a unified
    /// SaveChanges operation that commits all pending changes in a single transaction.
    /// </remarks>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The Entity Framework Core database context that manages the database connection and change tracking.
        /// </summary>
        private readonly AppDbContext _db;

        /// <summary>
        /// Repository for Product entities, providing inventory management operations.
        /// </summary>
        public IProductRepository Products { get; }

        /// <summary>
        /// Repository for Category entities, providing product classification operations.
        /// </summary>
        public ICategoryRepository Categories { get; }

        /// <summary>
        /// Repository for Sale entities, providing transaction management operations.
        /// </summary>
        public ISaleRepository Sales { get; }

        /// <summary>
        /// Repository for SaleCart entities, providing sale line item operations.
        /// </summary>
        public ISaleCartRepository SaleCarts { get; }

        /// <summary>
        /// Repository for User entities, providing authentication and user management operations.
        /// </summary>
        public IUserRepository Users { get; }

        /// <summary>
        /// Initializes a new instance of the UnitOfWork with the database context and all repositories.
        /// </summary>
        /// <param name="db">The Entity Framework Core database context</param>
        /// <param name="products">Repository for product operations</param>
        /// <param name="categories">Repository for category operations</param>
        /// <param name="sales">Repository for sale operations</param>
        /// <param name="saleCarts">Repository for sale cart operations</param>
        /// <param name="users">Repository for user operations</param>
        /// <remarks>
        /// Dependencies are injected to support dependency inversion and enable testing
        /// with mock repository implementations.
        /// </remarks>
        public UnitOfWork(AppDbContext db,
                          IProductRepository products,
                          ICategoryRepository categories,
                          ISaleRepository sales,
                          ISaleCartRepository saleCarts,
                          IUserRepository users)
        {
            _db = db;
            Products = products;
            Categories = categories;
            Sales = sales;
            SaleCarts = saleCarts;
            Users = users;
        }

        /// <summary>
        /// Saves all pending changes from all repositories to the database in a single transaction.
        /// </summary>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The number of entities written to the database</returns>
        /// <remarks>
        /// This method provides transactional consistency by:
        /// - Committing all repository changes in a single database transaction
        /// - Automatically handling transaction rollback on exceptions
        /// - Returning the number of affected entities for verification
        /// 
        /// All Add, Update, and Remove operations from all repositories are persisted
        /// when this method is called. If any operation fails, the entire transaction
        /// is rolled back to maintain data consistency.
        /// </remarks>
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        /// <summary>
        /// Asynchronously disposes the database context and releases all managed resources.
        /// </summary>
        /// <returns>A ValueTask representing the async disposal operation</returns>
        /// <remarks>
        /// Implements the IAsyncDisposable pattern to ensure proper cleanup of:
        /// - Database connections
        /// - Change tracking resources
        /// - Any pending transactions
        /// 
        /// This method should be called when the Unit of Work is no longer needed,
        /// typically through using statements or dependency injection container disposal.
        /// </remarks>
        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}