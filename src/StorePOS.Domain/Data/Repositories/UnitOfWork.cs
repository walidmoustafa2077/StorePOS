using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;

namespace StorePOS.Domain.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public ISaleRepository Sales { get; }
        public ISaleCartRepository SaleCarts { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext db,
                          IProductRepository products,
                          ICategoryRepository categories,
                          ISaleRepository sales,
                          ISaleCartRepository saleCarts,
                          IUserRepository users)
        {
            _db = db;
            Products = products;
            Categories = categories;
            Sales = sales;
            SaleCarts = saleCarts;
            Users = users;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

        public ValueTask DisposeAsync() => _db.DisposeAsync();
    }
}