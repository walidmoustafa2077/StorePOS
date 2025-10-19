using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Category repository implementation
    /// </summary>
    public class CategoryRepository : Repository<CategoryEntity>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CategoryEntity>> GetActiveAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CategoryEntity?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<CategoryEntity>> GetWithProductCountAsync()
        {
            // Note: This would require a join with Products table if we had the relationship properly set up
            // For now, we'll return categories and calculate product count separately if needed
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}