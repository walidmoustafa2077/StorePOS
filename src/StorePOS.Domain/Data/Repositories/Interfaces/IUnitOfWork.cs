namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ISaleRepository Sales { get; }
        ISaleCartRepository SaleCarts { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
