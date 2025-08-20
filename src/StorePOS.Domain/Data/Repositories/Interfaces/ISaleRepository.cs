using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        // Convenience method to load a Sale with its related lines and optionally product info
        Task<Sale?> GetWithLinesAsync(object id, bool includeProducts = false, CancellationToken ct = default);

        // Common query helpers
        Task<List<Sale>> ListWithLinesAsync(Expression<Func<Sale, bool>>? predicate = null,
                                            bool includeProducts = false,
                                            CancellationToken ct = default);
    }
}