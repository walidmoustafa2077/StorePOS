using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Extensions;

namespace StorePOS.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;

        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<List<ProductReadDto>> SearchAsync(string? q, CancellationToken ct)
        {
            var query = _uow.Products.Query()
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p =>
                    EF.Functions.Like(p.Name, $"%{q}%") ||
                    EF.Functions.Like(p.Sku, $"%{q}%") ||
                    p.Barcode == q);
            }

            return query
                .OrderBy(p => p.Name)
                .Take(100)
                .SelectReadDto()
                .ToListAsync(ct);
        }

        public Task<ProductReadDto?> GetByBarcodeAsync(string barcode, CancellationToken ct)
        {
            var trimmed = barcode?.Trim() ?? string.Empty;

            return _uow.Products.Query()
                .Where(p => p.IsActive && p.Barcode == trimmed)
                .SelectReadDto()
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ServiceResult<ProductReadDto>> CreateAsync(ProductCreateDto dto, CancellationToken ct)
        {
            var sku = dto.Sku?.Trim() ?? string.Empty;
            var name = dto.Name?.Trim() ?? string.Empty;
            var barcode = dto.Barcode?.Trim() ?? string.Empty;
            var categoryName = dto.Category?.Trim() ?? string.Empty;

            var errors = ValidateProduct(sku, barcode, name, categoryName, dto.Price, dto.Cost, dto.StockQty);
            if (errors.Count > 0) return ServiceResult<ProductReadDto>.Validation(errors);

            // Uniqueness checks
            if (await _uow.Products.ExistsAsync(p => p.Sku == sku, ct))
                return ServiceResult<ProductReadDto>.Conflict("SKU already exists.");

            if (!string.IsNullOrWhiteSpace(barcode) &&
                await _uow.Products.ExistsAsync(p => p.Barcode == barcode!, ct))
                return ServiceResult<ProductReadDto>.Conflict("Barcode already exists.");

            // Load category as tracked
            var category = await _uow.Categories.FirstOrDefaultAsync(c => c.Name == categoryName, asNoTracking: false, ct);
            if (category is null)
                return ServiceResult<ProductReadDto>.NotFound("Category does not exist.");

            var product = new Models.Product
            {
                Sku = sku,
                Barcode = barcode,
                Name = name,
                Category = category,
                Price = dto.Price,
                Cost = dto.Cost,
                StockQty = dto.StockQty,
                IsActive = dto.IsActive
            };

            await _uow.Products.AddAsync(product, ct);
            await _uow.SaveChangesAsync(ct);

            var id = product.Id;
            var created = await _uow.Products.Query()
                .Where(p => p.Id == id)
                .SelectReadDto()
                .FirstAsync(ct);

            return ServiceResult<ProductReadDto>.Created(created);
        }

        public async Task<ServiceResult<ProductReadDto>> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct)
        {
            var entity = await _uow.Products.GetByIdAsync(id, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<ProductReadDto>.NotFound("Product not found.");

            var sku = dto.Sku?.Trim() ?? string.Empty;
            var name = dto.Name?.Trim() ?? string.Empty;
            var barcode = dto.Barcode?.Trim() ?? string.Empty;
            var categoryName = dto.Category?.Trim() ?? string.Empty;

            var errors = ValidateProduct(sku, barcode, name, categoryName, dto.Price, dto.Cost, dto.StockQty);
            if (errors.Count > 0) return ServiceResult<ProductReadDto>.Validation(errors);

            // Uniqueness checks (exclude self)
            if (await _uow.Products.ExistsAsync(p => p.Sku == sku && EF.Property<int>(p, "Id") != id, ct))
                return ServiceResult<ProductReadDto>.Conflict("SKU already exists.");

            if (!string.IsNullOrWhiteSpace(barcode) &&
                await _uow.Products.ExistsAsync(p => p.Barcode == barcode! && EF.Property<int>(p, "Id") != id, ct))
                return ServiceResult<ProductReadDto>.Conflict("Barcode already exists.");

            var category = await _uow.Categories.FirstOrDefaultAsync(c => c.Name == categoryName, asNoTracking: false, ct);
            if (category is null)
                return ServiceResult<ProductReadDto>.NotFound("Category does not exist.");

            // Map changes
            entity.Sku = sku;
            entity.Barcode = barcode;
            entity.Name = name;
            entity.Category = category;
            entity.Price = dto.Price;
            entity.Cost = dto.Cost;
            entity.StockQty = dto.StockQty;
            entity.IsActive = dto.IsActive;

            await _uow.SaveChangesAsync(ct);

            var updated = await _uow.Products.Query()
                .Where(p => p.Id == id)
                .SelectReadDto()
                .FirstAsync(ct);

            return ServiceResult<ProductReadDto>.Ok(updated);
        }

        public async Task<ServiceResult<ProductReadDto>> UpdateStockAsync(int id, ProductStockUpdateDto dto, CancellationToken ct)
        {
            var entity = await _uow.Products.GetByIdAsync(id, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<ProductReadDto>.NotFound("Product not found.");

            var errors = ValidateUpdateStock(dto.amount, dto.stockUpdate);
            if (errors.Count > 0) return ServiceResult<ProductReadDto>.Validation(errors);
            StockUpdate update = (StockUpdate)Enum.Parse(typeof(StockUpdate), "dto.stockUpdate");

            // Apply stock update
            switch (update)
            {
                case StockUpdate.Increase:
                    entity.StockQty += dto.amount;
                    break;
                case StockUpdate.Decrease:
                    if (entity.StockQty < dto.amount)
                        return ServiceResult<ProductReadDto>.BadRequest("Insufficient stock to decrease.");
                    entity.StockQty -= dto.amount;
                    break;
            }

            await _uow.SaveChangesAsync(ct);

            var updated = await _uow.Products.Query()
                .Where(p => p.Id == id)
                .SelectReadDto()
                .FirstAsync(ct);

            return ServiceResult<ProductReadDto>.Ok(updated);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Products.GetByIdAsync(id, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<bool>.NotFound("Product not found.");

            _uow.Products.Remove(entity);
            await _uow.SaveChangesAsync(ct);

            return ServiceResult<bool>.NoContent();
        }

        private static Dictionary<string, string[]> ValidateProduct(string sku, string? barcode, string name, string category, decimal price, decimal cost, int stockQty)
        {
            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(sku)) errors["sku"] = new[] { "SKU is required." };
            if (string.IsNullOrWhiteSpace(name)) errors["name"] = new[] { "Name is required." };
            if (string.IsNullOrWhiteSpace(category)) errors["category"] = new[] { "Category is required." };
            if (price < 0) errors["price"] = new[] { "Price must be >= 0." };
            if (cost < 0) errors["cost"] = new[] { "Cost must be >= 0." };
            if (stockQty < 0) errors["stockQty"] = new[] { "StockQty must be >= 0." };
            return errors;
        }
        
        private static Dictionary<string, string[]> ValidateUpdateStock(int amount, string stockUpdate)
        {
            var errors = new Dictionary<string, string[]>();
            if (amount <= 0) errors["amount"] = new[] { "Amount must be greater than 0." };
            return errors;
        }
    }
}
