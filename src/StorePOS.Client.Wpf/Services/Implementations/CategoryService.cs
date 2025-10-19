using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data.Mapping;
using StorePOS.Client.Wpf.MVVM.Models;
using Microsoft.Extensions.Logging;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Implementation of ICategoryService providing category business operations
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync()
        {
            try
            {
                var entities = await _categoryRepository.GetAllAsync();
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all categories");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryModel>> GetActiveCategoriesAsync()
        {
            try
            {
                var entities = await _categoryRepository.GetActiveAsync();
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active categories");
                throw;
            }
        }

        public async Task<CategoryModel?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var entity = await _categoryRepository.GetByIdAsync(id);
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category by ID: {CategoryId}", id);
                throw;
            }
        }

        public async Task<CategoryModel?> GetCategoryByNameAsync(string name)
        {
            try
            {
                var entity = await _categoryRepository.GetByNameAsync(name);
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting category by name: {CategoryName}", name);
                throw;
            }
        }

        public async Task<ServiceResult<CategoryModel>> CreateCategoryAsync(CategoryModel category)
        {
            try
            {
                // Validate the category
                var validationResult = ValidateCategoryAsync(category, isUpdate: false);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<CategoryModel>.ValidationFailure(validationResult.ValidationErrors);
                }

                // Check if name already exists
                if (await IsCategoryNameExistsAsync(category.Name))
                {
                    return ServiceResult<CategoryModel>.ValidationFailure("Category name already exists");
                }

                // Create the entity
                var entity = category.ToEntity();
                entity.CreatedDate = DateTime.Now;
                entity.LastUpdated = DateTime.Now;

                var createdEntity = await _categoryRepository.AddAsync(entity);
                var createdModel = createdEntity.ToModel();

                _logger.LogInformation("Category created successfully: {CategoryName} (ID: {CategoryId})", 
                    createdModel.Name, createdModel.Id);

                return ServiceResult<CategoryModel>.Success(createdModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {CategoryName}", category.Name);
                return ServiceResult<CategoryModel>.Failure(ex);
            }
        }

        public async Task<ServiceResult<CategoryModel>> UpdateCategoryAsync(CategoryModel category)
        {
            try
            {
                // Validate the category
                var validationResult = ValidateCategoryAsync(category, isUpdate: true);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<CategoryModel>.ValidationFailure(validationResult.ValidationErrors);
                }

                // Check if category exists
                var existingEntity = await _categoryRepository.GetByIdAsync(category.Id);
                if (existingEntity == null)
                {
                    return ServiceResult<CategoryModel>.Failure("Category not found");
                }

                // Check if name already exists (excluding current category)
                if (await IsCategoryNameExistsAsync(category.Name, category.Id))
                {
                    return ServiceResult<CategoryModel>.ValidationFailure("Category name already exists");
                }

                // Update the entity
                existingEntity.UpdateFromModel(category);
                existingEntity.LastUpdated = DateTime.Now;

                var updatedEntity = await _categoryRepository.UpdateAsync(existingEntity);
                var updatedModel = updatedEntity.ToModel();

                _logger.LogInformation("Category updated successfully: {CategoryName} (ID: {CategoryId})", 
                    updatedModel.Name, updatedModel.Id);

                return ServiceResult<CategoryModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category: {CategoryId}", category.Id);
                return ServiceResult<CategoryModel>.Failure(ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                // Check if category exists
                var existingEntity = await _categoryRepository.GetByIdAsync(id);
                if (existingEntity == null)
                {
                    return ServiceResult<bool>.Failure("Category not found");
                }

                // Check if category has products
                var products = await _productRepository.FindAsync(p => p.Category == existingEntity.Name);
                if (products.Any())
                {
                    return ServiceResult<bool>.ValidationFailure("Cannot delete category that has products assigned to it");
                }

                // Soft delete by setting IsActive = false
                existingEntity.IsActive = false;
                existingEntity.LastUpdated = DateTime.Now;

                await _categoryRepository.UpdateAsync(existingEntity);

                _logger.LogInformation("Category soft deleted successfully: {CategoryName} (ID: {CategoryId})", 
                    existingEntity.Name, id);

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category: {CategoryId}", id);
                return ServiceResult<bool>.Failure(ex);
            }
        }

        public async Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetByNameAsync(name);
                
                if (existingCategory == null)
                    return false;

                if (excludeId.HasValue && existingCategory.Id == excludeId.Value)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if category name exists: {CategoryName}", name);
                throw;
            }
        }

        public async Task<IEnumerable<CategoryModel>> GetCategoriesWithProductCountAsync()
        {
            try
            {
                var entities = await _categoryRepository.GetWithProductCountAsync();
                var models = entities.Select(e => e.ToModel()).ToList();

                // Calculate product count for each category
                foreach (var model in models)
                {
                    var productCount = await _productRepository.CountAsync(p => p.Category == model.Name && p.IsActive);
                    model.ProductCount = productCount;
                }

                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories with product count");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryModel>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetActiveCategoriesAsync();
                }

                var entities = await _categoryRepository.FindAsync(c => 
                    (c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm)) && c.IsActive);
                
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching categories with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        private ServiceResult<bool> ValidateCategoryAsync(CategoryModel category, bool isUpdate)
        {
            var errors = new List<string>();

            // Basic validation
            if (string.IsNullOrWhiteSpace(category.Name))
                errors.Add("Category name is required");
            else if (category.Name.Length > 100)
                errors.Add("Category name cannot exceed 100 characters");

            if (!string.IsNullOrEmpty(category.Description) && category.Description.Length > 500)
                errors.Add("Category description cannot exceed 500 characters");

            if (!string.IsNullOrEmpty(category.Icon) && category.Icon.Length > 50)
                errors.Add("Category icon cannot exceed 50 characters");

            if (!string.IsNullOrEmpty(category.Color) && category.Color.Length > 20)
                errors.Add("Category color cannot exceed 20 characters");

            // For updates, ensure ID is valid
            if (isUpdate && category.Id <= 0)
                errors.Add("Valid category ID is required for updates");

            if (errors.Any())
            {
                return ServiceResult<bool>.ValidationFailure(errors);
            }

            return ServiceResult<bool>.Success(true);
        }
    }
}