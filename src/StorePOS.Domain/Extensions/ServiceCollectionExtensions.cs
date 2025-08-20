using Microsoft.Extensions.DependencyInjection;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Repositories;
using StorePOS.Domain.Services;

namespace StorePOS.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStorePOSRepositories(this IServiceCollection services)
        {
            // Assumes AppDbContext is already registered elsewhere with AddDbContext<AppDbContext>(...)
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<ISaleCartRepository, SaleCartRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection AddStorePOSServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
