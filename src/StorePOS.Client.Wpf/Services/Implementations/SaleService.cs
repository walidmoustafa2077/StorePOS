using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Data.Mapping;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Enums;
using Microsoft.Extensions.Logging;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Service implementation for sale business operations
    /// </summary>
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITransactionService _transactionService;
        private readonly IShiftService _shiftService;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository saleRepository,
            IProductRepository productRepository,
            ITransactionService transactionService,
            IShiftService shiftService,
            ILogger<SaleService> logger)
        {
            _saleRepository = saleRepository;
            _productRepository = productRepository;
            _transactionService = transactionService;
            _shiftService = shiftService;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleModel>> GetAllSalesAsync()
        {
            try
            {
                var sales = await _saleRepository.GetAllAsync();
                return sales.Select(SaleMapper.ToModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all sales");
                return Enumerable.Empty<SaleModel>();
            }
        }

        public async Task<SaleModel?> GetSaleByIdAsync(int id)
        {
            try
            {
                var sale = await _saleRepository.GetWithItemsAsync(id);
                return sale != null ? SaleMapper.ToModel(sale) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving sale with ID {id}");
                return null;
            }
        }

        public async Task<IEnumerable<SaleModel>> GetSalesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var sales = await _saleRepository.GetByDateRangeAsync(fromDate, toDate);
                return sales.Select(SaleMapper.ToModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving sales from {fromDate} to {toDate}");
                return Enumerable.Empty<SaleModel>();
            }
        }

        public async Task<IEnumerable<SaleModel>> GetSalesForCurrentShiftAsync()
        {
            try
            {
                var currentShift = await _shiftService.GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    _logger.LogWarning("No active shift found");
                    return Enumerable.Empty<SaleModel>();
                }

                var sales = await _saleRepository.GetByShiftIdAsync(currentShift.Id);
                return sales.Select(SaleMapper.ToModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales for current shift");
                return Enumerable.Empty<SaleModel>();
            }
        }

        public async Task<ServiceResult<SaleModel>>  CreateSaleAsync(SaleModel sale)
        {
            try
            {
                // Get current shift
                var currentShift = await _shiftService.GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return ServiceResult<SaleModel>.Failure("No active shift. Please start a shift first.");
                }

                // Generate sale number if not provided
                if (string.IsNullOrEmpty(sale.SaleNumber))
                {
                    sale.SaleNumber = await _saleRepository.GenerateNextSaleNumberAsync();
                }

                // Create sale entity
                var saleEntity = new SaleEntity
                {
                    SaleDate = sale.SaleDate,
                    SaleNumber = sale.SaleNumber,
                    Subtotal = sale.Subtotal,
                    TotalTax = sale.TotalTax,
                    TotalAmount = sale.TotalAmount,
                    AmountPaid = sale.AmountPaid,
                    ChangeAmount = sale.ChangeAmount,
                    PaymentMethod = sale.PaymentMethod,
                    CustomerName = sale.CustomerName,
                    Notes = sale.Notes,
                    CashierName = currentShift.CashierName,
                    ShiftId = currentShift.Id,
                    CreatedDate = DateTime.Now
                };

                // Add sale items
                foreach (var item in sale.Items)
                {
                    var saleItem = new SaleItemEntity
                    {
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        LineTotal = item.LineTotal,
                        TaxAmount = item.TaxAmount,
                        LineTotalWithTax = item.LineTotalWithTax,
                        Notes = item.Notes
                    };
                    saleEntity.Items.Add(saleItem);
                }

                // Save to database
                var savedSale = await _saleRepository.AddAsync(saleEntity);

                // Update product quantities
                foreach (var item in sale.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.Product.Id);
                    if (product != null)
                    {
                        var newQuantity = product.StockQuantity - item.Quantity;
                        await _productRepository.UpdateStockAsync(item.Product.Id, newQuantity);
                        _logger.LogInformation($"Updated stock for product {product.Name} (ID: {product.Id}): {product.StockQuantity} -> {newQuantity}");
                    }
                }

                // Create transaction record
                var transaction = new TransactionModel
                {
                    Type = TransactionType.Sale,
                    Amount = savedSale.TotalAmount,
                    Description = $"Sale #{savedSale.SaleNumber}",
                    Reference = savedSale.SaleNumber,
                    Timestamp = DateTime.Now,
                    Wallet = new WalletModel
                    {
                        Name = "Cash Register"
                    }
                };

                await _transactionService.AddTransactionAsync(transaction);

                _logger.LogInformation($"Sale {savedSale.SaleNumber} created successfully");

                return ServiceResult<SaleModel>.Success(SaleMapper.ToModel(savedSale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return ServiceResult<SaleModel>.Failure($"Error creating sale: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CancelSaleAsync(int saleId, string reason)
        {
            try
            {
                var sale = await _saleRepository.GetWithItemsAsync(saleId);
                if (sale == null)
                {
                    return ServiceResult<bool>.Failure($"Sale with ID {saleId} not found");
                }

                // Restore product quantities
                foreach (var item in sale.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        var newQuantity = product.StockQuantity + item.Quantity;
                        await _productRepository.UpdateStockAsync(item.ProductId, newQuantity);
                        _logger.LogInformation($"Restored stock for product {product.Name} (ID: {product.Id}): {product.StockQuantity} -> {newQuantity}");
                    }
                }

                // Add cancellation note
                sale.Notes = $"CANCELLED: {reason}. Original notes: {sale.Notes}";
                await _saleRepository.UpdateAsync(sale);

                // Create reversal transaction
                var transaction = new TransactionModel
                {
                    Type = TransactionType.Refund,
                    Amount = sale.TotalAmount, // Positive amount - will be subtracted from balance by TransactionService
                    Description = $"Sale #{sale.SaleNumber} cancelled - {reason}",
                    Reference = sale.SaleNumber,
                    Timestamp = DateTime.Now,
                    Wallet = new WalletModel
                    {
                        Name = "Cash Register"
                    }
                };

                await _transactionService.AddTransactionAsync(transaction);

                _logger.LogInformation($"Sale {sale.SaleNumber} cancelled. Reason: {reason}");

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling sale {saleId}");
                return ServiceResult<bool>.Failure($"Error cancelling sale: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> ProcessRefundAsync(int saleId, IEnumerable<RefundItemModel> refundItems, string reason, string refundedBy)
        {
            try
            {
                var sale = await _saleRepository.GetWithItemsAsync(saleId);
                if (sale == null)
                {
                    return ServiceResult<bool>.Failure($"Sale with ID {saleId} not found");
                }

                // Get current active shift
                var currentShift = await _shiftService.GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return ServiceResult<bool>.Failure("No active shift found. Please start a shift before processing refunds.");
                }

                decimal totalRefundAmount = 0;
                var refundItemsList = refundItems.ToList();

                // Process each refund item
                foreach (var refundItem in refundItemsList.Where(ri => ri.IsSelected && ri.RefundQuantity > 0))
                {
                    var saleItem = sale.Items.FirstOrDefault(si => si.Id == refundItem.SaleItemId);
                    if (saleItem == null)
                    {
                        _logger.LogWarning($"Sale item {refundItem.SaleItemId} not found in sale {saleId}");
                        continue;
                    }

                    // Validate refund quantity
                    var maxRefundable = saleItem.Quantity - saleItem.QuantityRefunded;
                    if (refundItem.RefundQuantity > maxRefundable)
                    {
                        return ServiceResult<bool>.Failure($"Cannot refund {refundItem.RefundQuantity} of {refundItem.ProductName}. Maximum refundable: {maxRefundable}");
                    }

                    // Calculate refund amount for this item (proportional to quantity)
                    var itemRefundAmount = (saleItem.LineTotal / saleItem.Quantity) * refundItem.RefundQuantity;

                    // Update sale item refund tracking
                    saleItem.QuantityRefunded += refundItem.RefundQuantity;
                    saleItem.RefundAmount += itemRefundAmount;
                    saleItem.IsFullyRefunded = saleItem.QuantityRefunded >= saleItem.Quantity;

                    totalRefundAmount += itemRefundAmount;

                    // Restore product stock
                    var product = await _productRepository.GetByIdAsync(saleItem.ProductId);
                    if (product != null)
                    {
                        var newQuantity = product.StockQuantity + refundItem.RefundQuantity;
                        await _productRepository.UpdateStockAsync(saleItem.ProductId, newQuantity);
                        _logger.LogInformation($"Restored stock for product {product.Name} (ID: {product.Id}): {product.StockQuantity} -> {newQuantity}");
                    }
                }

                // Update sale refund status
                sale.TotalRefundAmount += totalRefundAmount;
                sale.RefundDate = DateTime.Now;
                sale.RefundReason = reason;
                sale.RefundedBy = refundedBy;

                // Check if fully or partially refunded
                var allItemsFullyRefunded = sale.Items.All(item => item.IsFullyRefunded);
                sale.IsRefunded = allItemsFullyRefunded;
                sale.IsPartiallyRefunded = !allItemsFullyRefunded && sale.TotalRefundAmount > 0;

                // Update sale and all its items
                await _saleRepository.UpdateSaleWithItemsAsync(sale);

                // Create refund transaction in the active shift
                var transaction = new TransactionModel
                {
                    Type = TransactionType.Refund,
                    Amount = totalRefundAmount, // Positive amount - TransactionService will handle the negative balance change
                    Description = $"Refund for Sale #{sale.SaleNumber}" + (string.IsNullOrWhiteSpace(reason) ? "" : $" - {reason}"),
                    Reference = sale.SaleNumber,
                    Timestamp = DateTime.Now,
                    Wallet = new WalletModel
                    {
                        Name = "Cash Register"
                    }
                };

                await _transactionService.AddTransactionAsync(transaction);

                _logger.LogInformation($"Processed refund for sale {sale.SaleNumber}. Amount: ${totalRefundAmount:F2}, Refunded by: {refundedBy}");

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing refund for sale {saleId}");
                return ServiceResult<bool>.Failure($"Error processing refund: {ex.Message}");
            }
        }

        public async Task<SalesSummaryModel> GetTodaysSalesSummaryAsync()
        {
            var today = DateTime.Today;
            return await GetSalesSummaryAsync(today, today.AddDays(1).AddSeconds(-1));
        }

        public async Task<SalesSummaryModel> GetSalesSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var totalSalesCount = await _saleRepository.GetTotalSalesCountAsync(fromDate, toDate);
                var totalRevenue = await _saleRepository.GetTotalSalesAmountAsync(fromDate, toDate);
                var sales = await _saleRepository.GetByDateRangeAsync(fromDate, toDate);
                var itemsSold = sales.SelectMany(s => s.Items).Sum(i => i.Quantity);

                return new SalesSummaryModel
                {
                    TotalSales = totalSalesCount,
                    TotalRevenue = totalRevenue,
                    AverageSaleAmount = totalSalesCount > 0 ? totalRevenue / totalSalesCount : 0,
                    ItemsSold = itemsSold,
                    FromDate = fromDate,
                    ToDate = toDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting sales summary from {fromDate} to {toDate}");
                return new SalesSummaryModel
                {
                    FromDate = fromDate,
                    ToDate = toDate
                };
            }
        }

        public async Task<ServiceResult<SaleModel>> ProcessCheckoutAsync(IEnumerable<CartItemModel> cartItems, decimal totalAmount, string paymentMethod)
        {
            try
            {
                if (!cartItems.Any())
                {
                    return ServiceResult<SaleModel>.Failure("Cart is empty");
                }

                // Get current shift
                var currentShift = await _shiftService.GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return ServiceResult<SaleModel>.Failure("No active shift. Please start a shift first.");
                }

                // Generate sale number
                var saleNumber = await _saleRepository.GenerateNextSaleNumberAsync();

                // Create sale entity
                var saleEntity = new SaleEntity
                {
                    SaleDate = DateTime.Now,
                    SaleNumber = saleNumber,
                    Subtotal = totalAmount,
                    TotalTax = 0,
                    TotalAmount = totalAmount,
                    AmountPaid = totalAmount,
                    ChangeAmount = 0,
                    PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod, true),
                    CustomerName = string.Empty,
                    CashierName = currentShift.CashierName,
                    ShiftId = currentShift.Id,
                    CreatedDate = DateTime.Now
                };

                // Add sale items
                foreach (var cartItem in cartItems)
                {
                    var saleItem = new SaleItemEntity
                    {
                        ProductId = cartItem.Product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.UnitPrice,
                        Discount = 0,
                        LineTotal = cartItem.LineTotal,
                        TaxAmount = 0,
                        LineTotalWithTax = cartItem.LineTotal
                    };
                    saleEntity.Items.Add(saleItem);
                }

                // Save sale to database
                var savedSale = await _saleRepository.AddAsync(saleEntity);

                // Update product quantities
                foreach (var cartItem in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.Product.Id);
                    if (product != null)
                    {
                        var newQuantity = product.StockQuantity - cartItem.Quantity;
                        await _productRepository.UpdateStockAsync(cartItem.Product.Id, newQuantity);
                        _logger.LogInformation($"Updated stock for product {product.Name} (ID: {product.Id}): {product.StockQuantity} -> {newQuantity}");
                    }
                }

                // Create transaction record
                var transaction = new TransactionModel
                {
                    Type = TransactionType.Sale,
                    Amount = totalAmount,
                    Description = $"Sale #{saleNumber}",
                    Reference = saleNumber,
                    Timestamp = DateTime.Now,
                    Wallet = new WalletModel
                    {
                        Name = "Cash Register"
                    }
                };

                // Add transaction (this will also update wallet balance and fire SaleProcessed event)
                await _transactionService.AddTransactionAsync(transaction);

                // Convert to model
                var saleModel = new SaleModel
                {
                    Id = savedSale.Id,
                    SaleNumber = savedSale.SaleNumber,
                    SaleDate = savedSale.SaleDate,
                    Subtotal = savedSale.Subtotal,
                    TotalAmount = savedSale.TotalAmount,
                    AmountPaid = savedSale.AmountPaid,
                    PaymentMethod = savedSale.PaymentMethod,
                    CashierName = savedSale.CashierName
                };

                _logger.LogInformation($"Sale {saleNumber} processed successfully. Amount: {totalAmount:C}");

                return ServiceResult<SaleModel>.Success(saleModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout");
                return ServiceResult<SaleModel>.Failure($"Error processing checkout: {ex.Message}");
            }
        }

        public Task<ServiceResult<bool>> ValidateCartAsync(IEnumerable<CartItemModel> cartItems)
        {
            try
            {
                var itemsList = cartItems.ToList();

                if (!itemsList.Any())
                {
                    return Task.FromResult(ServiceResult<bool>.Failure("Cart is empty"));
                }

                // Validate each item
                foreach (var item in itemsList)
                {
                    if (item.Product == null)
                    {
                        return Task.FromResult(ServiceResult<bool>.Failure("Cart contains invalid product"));
                    }

                    if (item.Quantity <= 0)
                    {
                        return Task.FromResult(ServiceResult<bool>.Failure($"Invalid quantity for product {item.Product.Name}"));
                    }

                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        return Task.FromResult(ServiceResult<bool>.Failure($"Insufficient stock for {item.Product.Name}. Available: {item.Product.StockQuantity}"));
                    }

                    if (item.UnitPrice <= 0)
                    {
                        return Task.FromResult(ServiceResult<bool>.Failure($"Invalid price for product {item.Product.Name}"));
                    }
                }

                return Task.FromResult(ServiceResult<bool>.Success(true));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating cart");
                return Task.FromResult(ServiceResult<bool>.Failure($"Error validating cart: {ex.Message}"));
            }
        }

        public Task<CartTotalsModel> CalculateCartTotalsAsync(IEnumerable<CartItemModel> cartItems)
        {
            try
            {
                var itemsList = cartItems.ToList();

                var subtotal = itemsList.Sum(item => item.UnitPrice * item.Quantity);
                var totalDiscount = itemsList.Sum(item => item.Discount * item.Quantity);
                var totalTax = itemsList.Sum(item => item.TaxAmount);
                var grandTotal = itemsList.Sum(item => item.LineTotalWithTax);
                var totalItems = itemsList.Sum(item => item.Quantity);

                return Task.FromResult(new CartTotalsModel
                {
                    Subtotal = subtotal,
                    TotalDiscount = totalDiscount,
                    TotalTax = totalTax,
                    GrandTotal = grandTotal,
                    TotalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart totals");
                return Task.FromResult(new CartTotalsModel());
            }
        }

        public async Task<IEnumerable<ProductSalesModel>> GetBestSellingProductsAsync(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            try
            {
                var sales = await _saleRepository.GetByDateRangeAsync(fromDate, toDate);
                
                var productSales = sales
                    .SelectMany(s => s.Items)
                    .GroupBy(i => new { i.ProductId, i.Product.Name })
                    .Select(g => new ProductSalesModel
                    {
                        ProductId = g.Key.ProductId,
                        ProductName = g.Key.Name,
                        QuantitySold = g.Sum(i => i.Quantity),
                        TotalRevenue = g.Sum(i => i.LineTotalWithTax)
                    })
                    .OrderByDescending(p => p.QuantitySold)
                    .Take(topCount);

                return productSales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting best selling products from {fromDate} to {toDate}");
                return Enumerable.Empty<ProductSalesModel>();
            }
        }

        public async Task<ReceiptModel> GenerateReceiptAsync(int saleId)
        {
            try
            {
                var sale = await _saleRepository.GetWithItemsAsync(saleId);
                if (sale == null)
                {
                    _logger.LogWarning($"Sale with ID {saleId} not found for receipt generation");
                    return new ReceiptModel();
                }

                var receiptItems = sale.Items.Select(item => new CartItemModel
                {
                    Product = new ProductModel
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.UnitPrice
                    },
                    Quantity = item.Quantity,
                    Discount = item.Discount,
                    Notes = item.Notes
                }).ToList();

                return new ReceiptModel
                {
                    SaleId = sale.Id,
                    SaleDate = sale.SaleDate,
                    Items = receiptItems,
                    Subtotal = sale.Subtotal,
                    TotalDiscount = sale.Items.Sum(i => i.Discount * i.Quantity),
                    TotalTax = sale.TotalTax,
                    GrandTotal = sale.TotalAmount,
                    PaymentMethod = sale.PaymentMethod.ToString(),
                    CashierName = sale.CashierName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating receipt for sale {saleId}");
                return new ReceiptModel();
            }
        }
    }
}