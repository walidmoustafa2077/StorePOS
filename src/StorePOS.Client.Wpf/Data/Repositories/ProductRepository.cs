using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Product repository implementation
    /// </summary>
    public class ProductRepository : Repository<ProductEntity>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductEntity>> SearchAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllAsync();

            var searchLower = searchText.ToLower();
            return await _dbSet
                .Where(p => p.Name.ToLower().Contains(searchLower) ||
                           p.Description.ToLower().Contains(searchLower) ||
                           p.SKU.ToLower().Contains(searchLower) ||
                           p.Brand.ToLower().Contains(searchLower) ||
                           p.Category.ToLower().Contains(searchLower))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetLowStockAsync(int threshold = 10)
        {
            return await _dbSet
                .Where(p => p.StockQuantity <= threshold && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<ProductEntity?> GetBySkuAsync(string sku)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<ProductEntity?> GetByBarcodeAsync(string barcode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<bool> UpdateStockAsync(int productId, int newQuantity)
        {
            var product = await GetByIdAsync(productId);
            if (product == null)
                return false;

            product.StockQuantity = newQuantity;
            product.LastUpdated = DateTime.Now;
            
            await UpdateAsync(product);
            return true;
        }
    }
}