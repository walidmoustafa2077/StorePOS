using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.DTOs;
using StorePOS.Domain.Extensions;
using StorePOS.Domain.Models;
using StorePOS.Domain.Enums;

namespace StorePOS.Domain.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _uow;

        public SaleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task<List<SaleReadDto>> SearchAsync(DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken ct = default)
        {
            var query = _uow.Sales.Query()
                .Include(s => s.Carts)
                .ThenInclude(c => c.Product)
                .AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(s => s.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(s => s.CreatedAt <= to.Value);
            }

            return query
                .OrderByDescending(s => s.CreatedAt)
                .Take(100)
                .SelectReadDto()
                .ToListAsync(ct);
        }

        public async Task<SaleReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _uow.Sales.Query()
                .Where(s => s.Id == id)
                .Include(s => s.Carts)
                .ThenInclude(c => c.Product)
                .SelectReadDto()
                .FirstOrDefaultAsync(ct);
        }

        public async Task<ServiceResult<SaleReadDto>> CreateAsync(SaleCreateDto dto, CancellationToken ct = default)
        {
            var errors = ValidateSaleCreate(dto);
            if (errors.Count > 0) return ServiceResult<SaleReadDto>.Validation(errors);

            // Parse PaymentMethod from string
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
            {
                return ServiceResult<SaleReadDto>.BadRequest($"Invalid payment method: {dto.PaymentMethod}");
            }

            // Check if all products exist
            var productIds = dto.Carts.Select(c => c.ProductId).Distinct().ToList();
            var products = await _uow.Products.Query(asNoTracking: false)
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync(ct);

            if (products.Count != productIds.Count)
            {
                return ServiceResult<SaleReadDto>.BadRequest("One or more products not found or inactive.");
            }

            // Create sale carts with corrected prices
            var saleCarts = dto.Carts.Select(cart =>
            {
                var product = products.First(p => p.Id == cart.ProductId);
                var unitPrice = cart.UnitPrice > 0 ? cart.UnitPrice : product.Price;

                if (product.StockQty <= 0 || cart.Qty > product.StockQty)
                    return null;

                var qty = cart.Qty > 0 ? cart.Qty : 1;

                return new SaleCart
                {
                    ProductId = cart.ProductId,
                    Qty = qty,
                    UnitPrice = unitPrice,
                    LineTotal = qty * unitPrice
                };
            }).ToList();

            if (saleCarts.Any(c => c == null))
            {
                return ServiceResult<SaleReadDto>.BadRequest("One or more cart items are invalid.");
            }

            // Calculate totals using corrected prices
            var subtotal = saleCarts.Sum(saleCart => saleCart!.LineTotal);
            var total = subtotal - dto.Discount + dto.Tax;

            var sale = new Sale
            {
                CreatedAt = DateTimeOffset.UtcNow,
                Subtotal = subtotal,
                Discount = dto.Discount,
                Tax = dto.Tax,
                Total = total,
                PaidAmount = dto.PaidAmount,
                PaymentMethod = paymentMethod,
                Notes = dto.Notes,
                Status = SaleStatus.Pending,
                Carts = saleCarts!
            };

            await _uow.Sales.AddAsync(sale, ct);
            await _uow.SaveChangesAsync(ct);

            var created = await GetByIdAsync(sale.Id, ct);
            return ServiceResult<SaleReadDto>.Created(created!);
        }

        public async Task<ServiceResult<SaleReadDto>> UpdateAsync(int id, SaleUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _uow.Sales.GetByIdAsync(id, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<SaleReadDto>.NotFound("Sale not found.");

            if (entity.Status == SaleStatus.Completed)
            {
                return ServiceResult<SaleReadDto>.BadRequest("Cannot update completed sale.");
            }

            var errors = ValidateSaleUpdate(dto);
            if (errors.Count > 0) return ServiceResult<SaleReadDto>.Validation(errors);

            // Parse PaymentMethod from string
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
            {
                return ServiceResult<SaleReadDto>.BadRequest($"Invalid payment method: {dto.PaymentMethod}");
            }

            // Parse SaleStatus from string
            if (!Enum.TryParse<SaleStatus>(dto.Status, true, out var status))
            {
                return ServiceResult<SaleReadDto>.BadRequest($"Invalid status: {dto.Status}");
            }

            // Load existing sale carts
            await _uow.Sales.Query(asNoTracking: false)
                .Where(s => s.Id == id)
                .Include(s => s.Carts)
                .LoadAsync(ct);

            // Check if all products exist
            var productIds = dto.Carts.Select(c => c.ProductId).Distinct().ToList();
            var products = await _uow.Products.Query(asNoTracking: false)
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync(ct);

            if (products.Count != productIds.Count)
            {
                return ServiceResult<SaleReadDto>.BadRequest("One or more products not found or inactive.");
            }

            // Remove existing carts
            _uow.SaleCarts.RemoveRange(entity.Carts);

            // Create updated carts with corrected prices
            var updatedCarts = dto.Carts.Select(cart =>
            {
                var product = products.First(p => p.Id == cart.ProductId);
                var unitPrice = cart.UnitPrice > 0 ? cart.UnitPrice : product.Price;

                if (product.StockQty <= 0 || cart.Qty > product.StockQty)
                    return null;

                var qty = cart.Qty > 0 ? cart.Qty : 1;

                return new SaleCart
                {
                    SaleId = id,
                    ProductId = cart.ProductId,
                    Qty = qty,
                    UnitPrice = unitPrice,
                    LineTotal = qty * unitPrice
                };
            }).ToList();

            if (updatedCarts.Any(c => c == null))
            {
                return ServiceResult<SaleReadDto>.BadRequest("One or more cart items are invalid.");
            }

            // Calculate totals using corrected prices
            var subtotal = updatedCarts.Sum(updatedCart => updatedCart!.LineTotal);
            var total = subtotal - dto.Discount + dto.Tax;

            // Update sale
            entity.Subtotal = subtotal;
            entity.Discount = dto.Discount;
            entity.Tax = dto.Tax;
            entity.Total = total;
            entity.PaidAmount = dto.PaidAmount;
            entity.PaymentMethod = paymentMethod;
            entity.Notes = dto.Notes;
            entity.Status = status;
            entity.Carts = updatedCarts!;

            await _uow.SaveChangesAsync(ct);

            var updated = await GetByIdAsync(id, ct);
            return ServiceResult<SaleReadDto>.Ok(updated!);
        }

        public async Task<ServiceResult<SaleReadDto>> CompleteSaleAsync(int saleId, CancellationToken ct = default)
        {
            var sale = await _uow.Sales.GetWithLinesAsync(saleId, includeProducts: true, ct);
            if (sale is null) return ServiceResult<SaleReadDto>.NotFound("Sale not found.");

            if (sale.Status != SaleStatus.Pending)
            {
                return ServiceResult<SaleReadDto>.BadRequest("Only pending sales can be completed.");
            }

            // Check stock availability
            foreach (var cart in sale.Carts)
            {
                var product = await _uow.Products.GetByIdAsync(cart.ProductId, asNoTracking: false, ct);
                if (product is null || !product.IsActive)
                {
                    return ServiceResult<SaleReadDto>.BadRequest($"Product with ID {cart.ProductId} not found or inactive.");
                }

                if (product.StockQty < cart.Qty)
                {
                    return ServiceResult<SaleReadDto>.BadRequest($"Insufficient stock for product {product.Name}. Available: {product.StockQty}, Requested: {cart.Qty}");
                }
            }

            // Update stock quantities
            foreach (var cart in sale.Carts)
            {
                var product = await _uow.Products.GetByIdAsync(cart.ProductId, asNoTracking: false, ct);
                product!.StockQty -= cart.Qty;
                _uow.Products.Update(product);
            }

            // Update sale status
            var saleEntity = await _uow.Sales.GetByIdAsync(saleId, asNoTracking: false, ct);
            saleEntity!.Status = SaleStatus.Completed;
            _uow.Sales.Update(saleEntity);

            await _uow.SaveChangesAsync(ct);

            var updated = await GetByIdAsync(saleId, ct);
            return ServiceResult<SaleReadDto>.Ok(updated!);
        }

        public async Task<ServiceResult<SaleReadDto>> CancelSaleAsync(int saleId, CancellationToken ct = default)
        {
            var entity = await _uow.Sales.GetByIdAsync(saleId, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<SaleReadDto>.NotFound("Sale not found.");

            if (entity.Status == SaleStatus.Completed)
            {
                return ServiceResult<SaleReadDto>.BadRequest("Cannot cancel completed sale.");
            }

            if (entity.Status == SaleStatus.Cancelled)
            {
                return ServiceResult<SaleReadDto>.BadRequest("Sale is already cancelled.");
            }

            entity.Status = SaleStatus.Cancelled;
            _uow.Sales.Update(entity);
            await _uow.SaveChangesAsync(ct);

            var updated = await GetByIdAsync(saleId, ct);
            return ServiceResult<SaleReadDto>.Ok(updated!);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.Sales.GetByIdAsync(id, asNoTracking: false, ct);
            if (entity is null) return ServiceResult<bool>.NotFound("Sale not found.");

            if (entity.Status == SaleStatus.Completed)
            {
                return ServiceResult<bool>.BadRequest("Cannot delete completed sale.");
            }

            _uow.Sales.Remove(entity);
            await _uow.SaveChangesAsync(ct);

            return ServiceResult<bool>.NoContent();
        }

        private static Dictionary<string, string[]> ValidateSaleCreate(SaleCreateDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (dto.Carts == null || !dto.Carts.Any())
            {
                errors["carts"] = new[] { "At least one cart item is required." };
            }
            else
            {
                foreach (var cart in dto.Carts)
                {
                    if (cart.ProductId <= 0)
                        errors["carts"] = new[] { "Invalid product ID." };
                }
            }

            if (dto.Discount < 0) errors["discount"] = new[] { "Discount must be >= 0." };
            if (dto.Tax < 0) errors["tax"] = new[] { "Tax must be >= 0." };
            if (dto.PaidAmount < 0) errors["paidAmount"] = new[] { "Paid amount must be >= 0." };

            // Validate PaymentMethod string
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out _))
            {
                var validMethods = string.Join(", ", Enum.GetNames<PaymentMethod>());
                errors["paymentMethod"] = new[] { $"Invalid payment method. Valid options: {validMethods}" };
            }

            return errors;
        }

        private static Dictionary<string, string[]> ValidateSaleUpdate(SaleUpdateDto dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (dto.Carts == null || !dto.Carts.Any())
            {
                errors["carts"] = new[] { "At least one cart item is required." };
            }
            else
            {
                foreach (var cart in dto.Carts)
                {
                    if (cart.ProductId <= 0)
                        errors["carts"] = new[] { "Invalid product ID." };
                }
            }

            if (dto.Discount < 0) errors["discount"] = new[] { "Discount must be >= 0." };
            if (dto.Tax < 0) errors["tax"] = new[] { "Tax must be >= 0." };

            // Validate PaymentMethod string
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out _))
            {
                var validMethods = string.Join(", ", Enum.GetNames<PaymentMethod>());
                errors["paymentMethod"] = new[] { $"Invalid payment method. Valid options: {validMethods}" };
            }

            // Validate SaleStatus string
            if (!Enum.TryParse<SaleStatus>(dto.Status, true, out _))
            {
                var validStatuses = string.Join(", ", Enum.GetNames<SaleStatus>());
                errors["status"] = new[] { $"Invalid status. Valid options: {validStatuses}" };
            }

            return errors;
        }
    }
}
