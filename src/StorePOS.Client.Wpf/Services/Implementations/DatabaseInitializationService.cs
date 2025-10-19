using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StorePOS.Client.Wpf.Data;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Service to initialize and seed the database on application startup
    /// </summary>
    public class DatabaseInitializationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInitializationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting database initialization...");

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Initialize and seed the database
                await DatabaseSeeder.SeedAsync(context);

                _logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}