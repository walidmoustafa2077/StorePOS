using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data.Mapping;
using StorePOS.Client.Wpf.MVVM.Models;
using Microsoft.Extensions.Logging;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Implementation of IProductService providing product business operations
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductModel>> GetAllProductsAsync()
        {
            try
            {
                var entities = await _productRepository.GetAllAsync();
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products");
                throw;
            }
        }

        public async Task<IEnumerable<ProductModel>> GetActiveProductsAsync()
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.IsActive);
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active products");
                throw;
            }
        }

        public async Task<ProductModel?> GetProductByIdAsync(int id)
        {
            try
            {
                var entity = await _productRepository.GetByIdAsync(id);
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product by ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<ProductModel?> GetProductBySkuAsync(string sku)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.SKU == sku);
                var entity = entities.FirstOrDefault();
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product by SKU: {SKU}", sku);
                throw;
            }
        }

        public async Task<ProductModel?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.Barcode == barcode);
                var entity = entities.FirstOrDefault();
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product by barcode: {Barcode}", barcode);
                throw;
            }
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategoryAsync(string category)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.Category == category && p.IsActive);
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting products by category: {Category}", category);
                throw;
            }
        }

        public async Task<ServiceResult<ProductModel>> CreateProductAsync(ProductModel product)
        {
            try
            {
                // Validate the product
                var validationResult = await ValidateProductAsync(product, isUpdate: false);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<ProductModel>.ValidationFailure(validationResult.ValidationErrors);
                }

                // Check if SKU already exists
                if (!string.IsNullOrEmpty(product.SKU) && await IsSkuExistsAsync(product.SKU))
                {
                    return ServiceResult<ProductModel>.ValidationFailure("SKU already exists");
                }

                // Check if barcode already exists
                if (!string.IsNullOrEmpty(product.Barcode) && await IsBarcodeExistsAsync(product.Barcode))
                {
                    return ServiceResult<ProductModel>.ValidationFailure("Barcode already exists");
                }

                // Create the entity
                var entity = product.ToEntity();
                entity.CreatedDate = DateTime.Now;
                entity.LastUpdated = DateTime.Now;

                var createdEntity = await _productRepository.AddAsync(entity);
                var createdModel = createdEntity.ToModel();

                _logger.LogInformation("Product created successfully: {ProductName} (ID: {ProductId})", 
                    createdModel.Name, createdModel.Id);

                return ServiceResult<ProductModel>.Success(createdModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}", product.Name);
                return ServiceResult<ProductModel>.Failure(ex);
            }
        }

        public async Task<ServiceResult<ProductModel>> UpdateProductAsync(ProductModel product)
        {
            try
            {
                // Validate the product
                var validationResult = await ValidateProductAsync(product, isUpdate: true);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<ProductModel>.ValidationFailure(validationResult.ValidationErrors);
                }

                // Check if product exists
                var existingEntity = await _productRepository.GetByIdAsync(product.Id);
                if (existingEntity == null)
                {
                    return ServiceResult<ProductModel>.Failure("Product not found");
                }

                // Check if SKU already exists (excluding current product)
                if (!string.IsNullOrEmpty(product.SKU) && await IsSkuExistsAsync(product.SKU, product.Id))
                {
                    return ServiceResult<ProductModel>.ValidationFailure("SKU already exists");
                }

                // Check if barcode already exists (excluding current product)
                if (!string.IsNullOrEmpty(product.Barcode) && await IsBarcodeExistsAsync(product.Barcode, product.Id))
                {
                    return ServiceResult<ProductModel>.ValidationFailure("Barcode already exists");
                }

                // Update the entity
                existingEntity.UpdateFromModel(product);
                existingEntity.LastUpdated = DateTime.Now;

                var updatedEntity = await _productRepository.UpdateAsync(existingEntity);
                var updatedModel = updatedEntity.ToModel();

                _logger.LogInformation("Product updated successfully: {ProductName} (ID: {ProductId})", 
                    updatedModel.Name, updatedModel.Id);

                return ServiceResult<ProductModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product: {ProductId}", product.Id);
                return ServiceResult<ProductModel>.Failure(ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteProductAsync(int id)
        {
            try
            {
                // Check if product exists
                var existingEntity = await _productRepository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return ServiceResult<bool>.Failure("Product not found");
                }

                // Soft delete by setting IsActive = false
                existingEntity.IsActive = false;
                existingEntity.LastUpdated = DateTime.Now;

                await _productRepository.UpdateAsync(existingEntity);

                _logger.LogInformation("Product soft deleted successfully: {ProductName} (ID: {ProductId})", 
                    existingEntity.Name, id);

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product: {ProductId}", id);
                return ServiceResult<bool>.Failure(ex);
            }
        }

        public async Task<IEnumerable<ProductModel>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetActiveProductsAsync();
                }

                var entities = await _productRepository.FindAsync(p => 
                    (p.Name.Contains(searchTerm) || 
                     p.SKU.Contains(searchTerm) || 
                     p.Barcode.Contains(searchTerm) ||
                     p.Description.Contains(searchTerm)) && p.IsActive);
                
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching products with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.SKU == sku);
                var existingProduct = entities.FirstOrDefault();
                
                if (existingProduct == null)
                    return false;

                if (excludeId.HasValue && existingProduct.Id == excludeId.Value)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if SKU exists: {SKU}", sku);
                throw;
            }
        }

        public async Task<bool> IsBarcodeExistsAsync(string barcode, int? excludeId = null)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.Barcode == barcode);
                var existingProduct = entities.FirstOrDefault();
                
                if (existingProduct == null)
                    return false;

                if (excludeId.HasValue && existingProduct.Id == excludeId.Value)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if barcode exists: {Barcode}", barcode);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateStockAsync(int productId, int newQuantity)
        {
            try
            {
                var existingEntity = await _productRepository.GetByIdAsync(productId);
                if (existingEntity == null)
                {
                    return ServiceResult<bool>.Failure("Product not found");
                }

                if (newQuantity < 0)
                {
                    return ServiceResult<bool>.ValidationFailure("Stock quantity cannot be negative");
                }

                existingEntity.StockQuantity = newQuantity;
                existingEntity.LastUpdated = DateTime.Now;

                await _productRepository.UpdateAsync(existingEntity);

                _logger.LogInformation("Product stock updated: {ProductName} (ID: {ProductId}) - New Stock: {NewStock}", 
                    existingEntity.Name, productId, newQuantity);

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating stock for product: {ProductId}", productId);
                return ServiceResult<bool>.Failure(ex);
            }
        }

        public async Task<bool> HasSufficientStockAsync(int productId, int requiredQuantity)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                return product != null && product.StockQuantity >= requiredQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking stock for product: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductModel>> GetLowStockProductsAsync(int threshold = 10)
        {
            try
            {
                var entities = await _productRepository.FindAsync(p => p.StockQuantity <= threshold && p.IsActive);
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting low stock products with threshold: {Threshold}", threshold);
                throw;
            }
        }

        public decimal CalculateProfitMargin(ProductModel product)
        {
            if (product.Price <= 0 || product.Cost <= 0)
                return 0;

            return ((product.Price - product.Cost) / product.Price) * 100;
        }

        public decimal CalculateDiscountedPrice(ProductModel product)
        {
            if (product.Discount <= 0)
                return product.Price;

            return product.Price - (product.Price * (product.Discount / 100));
        }

        private async Task<ServiceResult<bool>> ValidateProductAsync(ProductModel product, bool isUpdate)
        {
            var errors = new List<string>();

            // Basic validation
            if (string.IsNullOrWhiteSpace(product.Name))
                errors.Add("Product name is required");
            else if (product.Name.Length > 255)
                errors.Add("Product name cannot exceed 255 characters");

            if (product.Price < 0)
                errors.Add("Product price cannot be negative");

            if (product.Cost < 0)
                errors.Add("Product cost cannot be negative");

            if (product.StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative");

            if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 1000)
                errors.Add("Product description cannot exceed 1000 characters");

            if (!string.IsNullOrEmpty(product.SKU) && product.SKU.Length > 50)
                errors.Add("SKU cannot exceed 50 characters");

            if (!string.IsNullOrEmpty(product.Barcode) && product.Barcode.Length > 50)
                errors.Add("Barcode cannot exceed 50 characters");

            if (product.Discount < 0 || product.Discount > 100)
                errors.Add("Discount must be between 0 and 100");

            if (product.TaxRate < 0 || product.TaxRate > 100)
                errors.Add("Tax rate must be between 0 and 100");

            // For updates, ensure ID is valid
            if (isUpdate && product.Id <= 0)
                errors.Add("Valid product ID is required for updates");

            // Validate category exists if specified
            if (!string.IsNullOrEmpty(product.Category))
            {
                try
                {
                    var categoryExists = await _categoryRepository.GetByNameAsync(product.Category);
                    if (categoryExists == null)
                        errors.Add("Specified category does not exist");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not validate category existence: {Category}", product.Category);
                }
            }

            if (errors.Any())
            {
                return ServiceResult<bool>.ValidationFailure(errors);
            }

            return ServiceResult<bool>.Success(true);
        }
    }
}