using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext db) : base(db) { }

        public async Task<Product?> GetAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return null;

            query = query.Trim();

            // 1) Exact match priority: SKU, Barcode, Name
            var exact = await _set.AsNoTracking()
                                  .Where(p => p.Sku == query || p.Barcode == query || p.Name == query)
                                  .OrderByDescending(p => p.Sku == query)
                                  .ThenByDescending(p => p.Barcode == query)
                                  .ThenByDescending(p => p.Name == query)
                                  .FirstOrDefaultAsync(ct);

            if (exact is not null)
                return exact;

            // 2) Fallback: partial match across fields
            return await _set.AsNoTracking()
                             .Where(p => p.Sku.Contains(query) || p.Barcode.Contains(query) || p.Name.Contains(query))
                             .OrderBy(p => p.Name)
                             .FirstOrDefaultAsync(ct);
        }
    }
}