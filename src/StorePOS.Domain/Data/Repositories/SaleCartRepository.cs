using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Implementation of ISaleCartRepository providing Entity Framework-based data access for SaleCart entities.
    /// Inherits all functionality from GenericRepository without additional specialized operations.
    /// </summary>
    public class SaleCartRepository : GenericRepository<SaleCart>, ISaleCartRepository
    {
        public SaleCartRepository(AppDbContext db) : base(db) { }
    }
}