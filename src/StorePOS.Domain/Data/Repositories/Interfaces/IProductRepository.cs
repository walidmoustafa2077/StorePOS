using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // One method to get a product by any field: SKU, Barcode, or Name.
        // - Tries exact matches first.
        // - If no exact match, falls back to partial matches and returns the best match (or null).
        Task<Product?> GetAsync(string query, CancellationToken ct = default);
    }
}