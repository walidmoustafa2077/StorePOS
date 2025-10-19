using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data
{
    /// <summary>
    /// Database seeder for initial data
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Seeds the database with initial data
        /// </summary>
        public static async Task SeedAsync(AppDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Categories first
            await SeedCategoriesAsync(context);

            // Seed Wallets
            await SeedWalletsAsync(context);

            // Seed Products from images
            await SeedProductsAsync(context);

            // Save all changes
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return; // Categories already seeded

            var categories = new List<CategoryEntity>
            {
                new() { Name = "Cables", Description = "Various types of cables and connectors", Icon = "cable", Color = "#FF6B35", IsActive = true },
                new() { Name = "Chargers", Description = "Phone and device chargers", Icon = "power", Color = "#F7931E", IsActive = true },
                new() { Name = "Headphones", Description = "Audio headphones and earphones", Icon = "headphones", Color = "#FFD23F", IsActive = true },
                new() { Name = "Speakers", Description = "Bluetooth and wired speakers", Icon = "speaker", Color = "#06FFA5", IsActive = true },
                new() { Name = "Keyboards", Description = "Computer keyboards", Icon = "keyboard", Color = "#118AB2", IsActive = true },
                new() { Name = "Mice", Description = "Computer mice and pointing devices", Icon = "mouse", Color = "#073B4C", IsActive = true },
                new() { Name = "Smartwatches", Description = "Smart watches and fitness trackers", Icon = "watch", Color = "#EF476F", IsActive = true },
                new() { Name = "Storage", Description = "USB drives and storage devices", Icon = "harddisk", Color = "#8338EC", IsActive = true },
                new() { Name = "Accessories", Description = "Phone and computer accessories", Icon = "tools", Color = "#3A86FF", IsActive = true },
                new() { Name = "Mobiles", Description = "Mobile phones and smartphones", Icon = "phone", Color = "#FF006E", IsActive = true },
                new() { Name = "Electronics", Description = "General electronics", Icon = "chip", Color = "#8B5CF6", IsActive = true },
                new() { Name = "Audio", Description = "Audio equipment and devices", Icon = "music", Color = "#06D6A0", IsActive = true },
                new() { Name = "Computers", Description = "Computer hardware and components", Icon = "desktop", Color = "#F72585", IsActive = true },
                new() { Name = "Wearables", Description = "Wearable technology devices", Icon = "wearable", Color = "#4CC9F0", IsActive = true }
            };

            await context.Categories.AddRangeAsync(categories);
        }

        private static async Task SeedProductsAsync(AppDbContext context)
        {
            if (await context.Products.AnyAsync())
                return; // Products already seeded

            var products = CreateProductsFromImages();
            await context.Products.AddRangeAsync(products);
        }

        private static List<ProductEntity> CreateProductsFromImages()
        {
            var path = @"D:\Side Projects\Products Pictures";

            if (System.IO.Directory.Exists(path))
            {
                var imageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp"
                };

                var images = System.IO.Directory.EnumerateFiles(path)
                    .Where(file => imageExtensions.Contains(System.IO.Path.GetExtension(file)))
                    .ToList();

                var random = new Random();
                int id = 1;

                return images.Select(file =>
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(file);

                    // Convert ProductModel to ProductEntity
                    return new ProductEntity
                    {
                        Name = fileName,
                        Description = $"Description for {fileName}",
                        Price = (decimal)(random.Next(50, 500) + random.NextDouble()),
                        Cost = (decimal)(random.Next(20, 100) + random.NextDouble()),
                        StockQuantity = random.Next(0, 50),
                        Category = DetectCategory(fileName),
                        SKU = $"SKU{id++:0000}",
                        Barcode = Guid.NewGuid().ToString("N").Substring(0, 12),
                        ImageUrl = file,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        Brand = fileName.Split(' ').FirstOrDefault() ?? "Generic",
                        Supplier = "Mock Supplier",
                        Weight = (decimal)(random.NextDouble() * 2),
                        Unit = "pcs",
                        Discount = random.Next(0, 20),
                        IsTaxable = true,
                        TaxRate = 0m
                    };
                }).ToList();
            }
            else
            {
                // Fallback: Create sample products when the image directory doesn't exist
                return CreateSampleProducts();
            }
        }

        private static string DetectCategory(string fileName)
        {
            fileName = fileName.ToLowerInvariant();

            if (fileName.Contains("cable")) return "Cables";
            if (fileName.Contains("charger")) return "Chargers";
            if (fileName.Contains("headphone") || fileName.Contains("ear")) return "Headphones";
            if (fileName.Contains("speaker")) return "Speakers";
            if (fileName.Contains("keyboard")) return "Keyboards";
            if (fileName.Contains("mouse")) return "Mice";
            if (fileName.Contains("smartwatch") || fileName.Contains("watch")) return "Smartwatches";
            if (fileName.Contains("flash drive") || fileName.Contains("usb")) return "Storage";
            if (fileName.Contains("stand") || fileName.Contains("holder")) return "Accessories";
            if (fileName.Contains("mobile")) return "Mobiles";

            // fallback: pick a random one from common categories
            string[] fallbackCategories =
            {
                "Accessories", "Electronics", "Audio", "Computers", "Wearables"
            };
            return fallbackCategories[new Random().Next(fallbackCategories.Length)];
        }

        private static List<ProductEntity> CreateSampleProducts()
        {
            var random = new Random();
            var sampleProducts = new List<ProductEntity>();

            var productData = new[]
            {
                ("USB-C Cable", "Cables", "High-quality USB-C charging cable", 15.99m, 8.50m),
                ("Lightning Cable", "Cables", "Apple Lightning charging cable", 19.99m, 10.00m),
                ("Wireless Charger", "Chargers", "Fast wireless charging pad", 29.99m, 15.00m),
                ("Phone Stand", "Accessories", "Adjustable phone stand", 12.99m, 6.00m),
                ("Bluetooth Headphones", "Headphones", "Noise-cancelling headphones", 89.99m, 45.00m),
                ("Wireless Mouse", "Mice", "Ergonomic wireless mouse", 24.99m, 12.00m),
                ("Mechanical Keyboard", "Keyboards", "RGB mechanical keyboard", 79.99m, 40.00m),
                ("USB Flash Drive 32GB", "Storage", "High-speed USB 3.0 flash drive", 14.99m, 7.00m),
                ("Portable Speaker", "Speakers", "Waterproof Bluetooth speaker", 49.99m, 25.00m),
                ("Smart Watch", "Smartwatches", "Fitness tracking smartwatch", 199.99m, 100.00m)
            };

            for (int i = 0; i < productData.Length; i++)
            {
                var (name, category, description, price, cost) = productData[i];
                
                sampleProducts.Add(new ProductEntity
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    Cost = cost,
                    StockQuantity = random.Next(10, 100),
                    Category = category,
                    SKU = $"SKU{(i + 1):0000}",
                    Barcode = Guid.NewGuid().ToString("N").Substring(0, 12),
                    ImageUrl = string.Empty,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    Brand = "Generic",
                    Supplier = "Sample Supplier",
                    Weight = (decimal)(random.NextDouble() * 2),
                    Unit = "pcs",
                    Discount = random.Next(0, 15),
                    IsTaxable = true,
                    TaxRate = 10m
                });
            }

            return sampleProducts;
        }

        private static async Task SeedWalletsAsync(AppDbContext context)
        {
            if (await context.Wallets.AnyAsync())
                return; // Wallets already seeded

            var wallets = new List<WalletEntity>
            {
                new() 
                { 
                    Name = "Cash Register", 
                    Description = "Physical cash payments and register", 
                    Balance = 0, 
                    IsActive = true 
                },
                new() 
                { 
                    Name = "Momken Wallet", 
                    Description = "Momken digital payment wallet", 
                    Balance = 0, 
                    IsActive = true 
                },
                new() 
                { 
                    Name = "Basata Wallet", 
                    Description = "Basata digital payment system", 
                    Balance = 0, 
                    IsActive = true 
                },
                new() 
                { 
                    Name = "Fawry Wallet", 
                    Description = "Fawry electronic payment platform", 
                    Balance = 0, 
                    IsActive = true 
                },
                new() 
                { 
                    Name = "Vodafone Cash", 
                    Description = "Vodafone mobile payment service", 
                    Balance = 0, 
                    IsActive = true 
                }
            };

            await context.Wallets.AddRangeAsync(wallets);
        }
    }
}