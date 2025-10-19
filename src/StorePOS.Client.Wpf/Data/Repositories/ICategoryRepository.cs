using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Category operations
    /// </summary>
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        Task<IEnumerable<CategoryEntity>> GetActiveAsync();
        Task<CategoryEntity?> GetByNameAsync(string name);
        Task<IEnumerable<CategoryEntity>> GetWithProductCountAsync();
    }
}