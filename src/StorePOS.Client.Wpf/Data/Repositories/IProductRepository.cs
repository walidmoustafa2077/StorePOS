using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Product operations
    /// </summary>
    public interface IProductRepository : IRepository<ProductEntity>
    {
        Task<IEnumerable<ProductEntity>> SearchAsync(string searchText);
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(string category);
        Task<IEnumerable<ProductEntity>> GetLowStockAsync(int threshold = 10);
        Task<IEnumerable<ProductEntity>> GetActiveProductsAsync();
        Task<ProductEntity?> GetBySkuAsync(string sku);
        Task<ProductEntity?> GetByBarcodeAsync(string barcode);
        Task<bool> UpdateStockAsync(int productId, int newQuantity);
    }
}