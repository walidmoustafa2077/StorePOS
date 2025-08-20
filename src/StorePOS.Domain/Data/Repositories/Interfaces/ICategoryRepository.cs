using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}