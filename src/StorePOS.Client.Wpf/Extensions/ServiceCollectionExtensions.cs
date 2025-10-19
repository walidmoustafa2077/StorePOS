using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.MVVM.ViewModels;
using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using StorePOS.Client.Wpf.MVVM.Views;
using StorePOS.Client.Wpf.MVVM.Views.HomeViews;
using StorePOS.Client.Wpf.Services;
using StorePOS.Client.Wpf.Services.Implementations;
using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data;

namespace StorePOS.Client.Wpf.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services with the dependency injection container
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbDirectory = System.IO.Path.Combine(appDataPath, "StorePOS");
            System.IO.Directory.CreateDirectory(dbDirectory);
            var dbPath = System.IO.Path.Combine(dbDirectory, "storepos.db");
            options.UseSqlite($"Data Source={dbPath}");
        });

        // Register database initialization service
        services.AddHostedService<DatabaseInitializationService>();

        // Register core services
        services.AddSingleton<IMainWindowService, MainWindowService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDebounceService, DebounceService>();

        // Register business services
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IProductFilterService, ProductFilterService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ITransactionService, TransactionService>();

        // Register repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<HomeViewModel>();

        // Register Home ViewModels (sub-ViewModels within HomeView)
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<CashierViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<SalesViewModel>();
        services.AddTransient<SettingsViewModel>();

        // Register Views/Windows
        services.AddTransient<MainWindow>();
        services.AddTransient<LoginView>();
        services.AddTransient<HomeView>();

        // Register Home Views (sub-views within HomeView)
        services.AddTransient<DashboardView>();
        services.AddTransient<CashierView>();
        services.AddTransient<ProductsView>();
        services.AddTransient<InventoryView>();
        services.AddTransient<SalesView>();
        services.AddTransient<SettingsView>();

        return services;
    }
}
