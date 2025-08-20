using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    public class SaleCartRepository : GenericRepository<SaleCart>, ISaleCartRepository
    {
        public SaleCartRepository(AppDbContext db) : base(db) { }
    }
}