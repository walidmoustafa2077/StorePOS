using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service interface for category business operations
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        Task<IEnumerable<CategoryModel>> GetAllCategoriesAsync();

        /// <summary>
        /// Get only active categories
        /// </summary>
        Task<IEnumerable<CategoryModel>> GetActiveCategoriesAsync();

        /// <summary>
        /// Get category by ID
        /// </summary>
        Task<CategoryModel?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Get category by name
        /// </summary>
        Task<CategoryModel?> GetCategoryByNameAsync(string name);

        /// <summary>
        /// Create a new category
        /// </summary>
        Task<ServiceResult<CategoryModel>> CreateCategoryAsync(CategoryModel category);

        /// <summary>
        /// Update an existing category
        /// </summary>
        Task<ServiceResult<CategoryModel>> UpdateCategoryAsync(CategoryModel category);

        /// <summary>
        /// Delete a category (soft delete by setting IsActive = false)
        /// </summary>
        Task<ServiceResult<bool>> DeleteCategoryAsync(int id);

        /// <summary>
        /// Check if category name already exists
        /// </summary>
        Task<bool> IsCategoryNameExistsAsync(string name, int? excludeId = null);

        /// <summary>
        /// Get categories with product count
        /// </summary>
        Task<IEnumerable<CategoryModel>> GetCategoriesWithProductCountAsync();

        /// <summary>
        /// Search categories by name or description
        /// </summary>
        Task<IEnumerable<CategoryModel>> SearchCategoriesAsync(string searchTerm);
    }
}