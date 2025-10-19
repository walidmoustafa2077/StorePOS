using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Data.Mapping
{
    /// <summary>
    /// Mapper for converting between SaleEntity and SaleModel
    /// </summary>
    public static class SaleMapper
    {
        /// <summary>
        /// Convert SaleEntity to SaleModel
        /// </summary>
        public static SaleModel ToModel(SaleEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var model = new SaleModel
            {
                Id = entity.Id,
                SaleNumber = entity.SaleNumber,
                SaleDate = entity.SaleDate,
                Subtotal = entity.Subtotal,
                TotalTax = entity.TotalTax,
                TotalAmount = entity.TotalAmount,
                AmountPaid = entity.AmountPaid,
                ChangeAmount = entity.ChangeAmount,
                PaymentMethod = (Enums.PaymentMethod)(int)entity.PaymentMethod,
                CustomerName = entity.CustomerName,
                Notes = entity.Notes,
                CashierName = entity.CashierName,
                // Refund tracking fields
                IsRefunded = entity.IsRefunded,
                IsPartiallyRefunded = entity.IsPartiallyRefunded,
                TotalRefundAmount = entity.TotalRefundAmount,
                RefundDate = entity.RefundDate,
                RefundReason = entity.RefundReason,
                RefundedBy = entity.RefundedBy
            };

            // Convert sale items
            if (entity.Items?.Any() == true)
            {
                model.Items = entity.Items.Select(item => new CartItemModel
                {
                    Id = item.Id, // Preserve the SaleItem ID
                    Product = new ProductModel
                    {
                        Id = item.Product?.Id ?? 0,
                        Name = item.Product?.Name ?? string.Empty,
                        Price = item.Product?.Price ?? 0,
                        SKU = item.Product?.SKU ?? string.Empty,
                        Barcode = item.Product?.Barcode ?? string.Empty,
                        Category = item.Product?.Category ?? string.Empty,
                        Description = item.Product?.Description ?? string.Empty,
                        StockQuantity = item.Product?.StockQuantity ?? 0
                    },
                    Quantity = item.Quantity,
                    // Refund tracking fields
                    QuantityRefunded = item.QuantityRefunded,
                    RefundAmount = item.RefundAmount,
                    IsFullyRefunded = item.IsFullyRefunded,
                    // Note: UnitPrice and TaxAmount are read-only calculated properties
                    // We don't set them directly as they're calculated from Product.Price and other values
                }).ToList();
            }

            return model;
        }

        /// <summary>
        /// Convert SaleModel to SaleEntity
        /// </summary>
        public static SaleEntity ToEntity(SaleModel model, int? shiftId = null)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // Validate shift ID
            if (shiftId == null || shiftId <= 0)
            {
                throw new InvalidOperationException("Cannot create sale without a valid shift. Please ensure a shift is active.");
            }

            var entity = new SaleEntity
            {
                Id = model.Id,
                SaleNumber = model.SaleNumber,
                SaleDate = model.SaleDate,
                Subtotal = model.Subtotal,
                TotalTax = model.TotalTax,
                TotalAmount = model.TotalAmount,
                AmountPaid = model.AmountPaid,
                ChangeAmount = model.ChangeAmount,
                PaymentMethod = (PaymentMethod)(int)model.PaymentMethod,
                CustomerName = model.CustomerName,
                Notes = model.Notes,
                CashierName = model.CashierName,
                CreatedDate = DateTime.Now,
                ShiftId = shiftId
            };

            // Convert sale items with validation
            if (model.Items?.Any() == true)
            {
                var saleItems = new List<SaleItemEntity>();
                
                foreach (var item in model.Items)
                {
                    // Validate product ID
                    if (item.Product == null)
                    {
                        throw new InvalidOperationException($"Cart item has no product information.");
                    }
                    
                    if (item.Product.Id <= 0)
                    {
                        throw new InvalidOperationException(
                            $"Product '{item.Product.Name}' has an invalid ID ({item.Product.Id}). " +
                            $"Products must be loaded from the database before they can be sold. " +
                            $"Please ensure you're using real products from the product catalog.");
                    }

                    saleItems.Add(new SaleItemEntity
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        LineTotal = item.LineTotal,
                        TaxAmount = item.TaxAmount,
                        LineTotalWithTax = item.LineTotalWithTax,
                        Notes = item.Notes
                    });
                }
                
                entity.Items = saleItems;
            }

            return entity;
        }

        /// <summary>
        /// Update existing SaleEntity from SaleModel
        /// </summary>
        public static void UpdateEntity(SaleEntity entity, SaleModel model)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (model == null) throw new ArgumentNullException(nameof(model));

            entity.SaleNumber = model.SaleNumber;
            entity.Subtotal = model.Subtotal;
            entity.TotalTax = model.TotalTax;
            entity.TotalAmount = model.TotalAmount;
            entity.AmountPaid = model.AmountPaid;
            entity.ChangeAmount = model.ChangeAmount;
            entity.PaymentMethod = (PaymentMethod)(int)model.PaymentMethod;
            entity.CustomerName = model.CustomerName;
            entity.Notes = model.Notes;
            entity.CashierName = model.CashierName;
        }
    }
}