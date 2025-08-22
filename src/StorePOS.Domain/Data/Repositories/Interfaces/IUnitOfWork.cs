namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Unit of Work interface that defines the contract for coordinating repository operations 
    /// and managing database transactions across multiple entities.
    /// </summary>
    /// <remarks>
    /// The Unit of Work pattern provides:
    /// - Transactional consistency across multiple repository operations
    /// - Single point of commit/rollback for complex business operations  
    /// - Reduced database round trips through batched operations
    /// - Proper resource management through IAsyncDisposable
    /// - Centralized access to all domain repositories
    /// 
    /// This interface ensures that all changes from multiple repositories
    /// are committed or rolled back together, maintaining data integrity
    /// across complex business operations.
    /// </remarks>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Gets the repository for Product entities, providing inventory management operations.
        /// </summary>
        IProductRepository Products { get; }

        /// <summary>
        /// Gets the repository for Category entities, providing product classification operations.
        /// </summary>
        ICategoryRepository Categories { get; }

        /// <summary>
        /// Gets the repository for Sale entities, providing transaction management operations.
        /// </summary>
        ISaleRepository Sales { get; }

        /// <summary>
        /// Gets the repository for SaleCart entities, providing sale line item operations.
        /// </summary>
        ISaleCartRepository SaleCarts { get; }

        /// <summary>
        /// Gets the repository for User entities, providing authentication and user management operations.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Saves all pending changes from all repositories to the database in a single transaction.
        /// </summary>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The number of entities written to the database</returns>
        /// <remarks>
        /// This method coordinates all repository changes into a single database transaction:
        /// - All Add, Update, and Remove operations are committed together
        /// - If any operation fails, the entire transaction is rolled back
        /// - Returns the count of affected entities for verification
        /// - Supports cancellation for responsive operation handling
        /// 
        /// This ensures ACID properties are maintained across complex operations
        /// involving multiple entities and repositories.
        /// </remarks>
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
