using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Implementation of ICategoryRepository providing Entity Framework-based data access for Category entities.
    /// Includes optimized name-based lookup operations for category management scenarios.
    /// </summary>
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext db) : base(db) { }

        /// <inheritdoc />
        public Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
            => _set.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name, ct);
    }
}