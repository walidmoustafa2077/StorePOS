using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for SaleCart entities representing individual line items within sales transactions.
    /// Inherits all standard CRUD operations from the generic repository without additional specialized methods.
    /// </summary>
    /// <remarks>
    /// SaleCart entities represent the many-to-many relationship between Sales and Products with quantity information.
    /// This repository primarily supports:
    /// - Cart line creation during sale processing
    /// - Inventory tracking and reporting
    /// - Sales analytics by product
    /// 
    /// Complex cart operations are typically handled through the parent Sale repository
    /// using Entity Framework's navigation properties and Include operations.
    /// </remarks>
    public interface ISaleCartRepository : IGenericRepository<SaleCart>
    {
    }
}
