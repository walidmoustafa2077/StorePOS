using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext db) : base(db) { }

        public Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
            => _set.AsNoTracking().FirstOrDefaultAsync(c => c.Name == name, ct);
    }
}