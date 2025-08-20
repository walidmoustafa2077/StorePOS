using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext db) : base(db) { }

        public async Task<Sale?> GetWithLinesAsync(object id, bool includeProducts = false, CancellationToken ct = default)
        {
            IQueryable<Sale> query = _set.AsNoTracking();

            if (includeProducts)
            {
                query = query
                    .Include(s => s.Carts)
                    .ThenInclude(c => c.Product);
            }
            else
            {
                query = query.Include(s => s.Carts);
            }

            return await query.FirstOrDefaultAsync(s => EF.Property<object>(s, "Id")!.Equals(id), ct);
        }

        public Task<List<Sale>> ListWithLinesAsync(Expression<Func<Sale, bool>>? predicate = null,
                                                   bool includeProducts = false,
                                                   CancellationToken ct = default)
        {
            IQueryable<Sale> query = _set.AsNoTracking();

            if (includeProducts)
            {
                query = query
                    .Include(s => s.Carts)
                    .ThenInclude(c => c.Product);
            }
            else
            {
                query = query.Include(s => s.Carts);
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return query.ToListAsync(ct);
        }
    }
}