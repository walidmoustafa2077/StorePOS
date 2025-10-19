using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Data.Mapping
{
    /// <summary>
    /// Extension methods for mapping between CategoryEntity and CategoryModel
    /// </summary>
    public static class CategoryMappingExtensions
    {
        /// <summary>
        /// Convert CategoryEntity to CategoryModel
        /// </summary>
        public static CategoryModel ToModel(this CategoryEntity entity)
        {
            return new CategoryModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Icon = entity.Icon,
                Color = entity.Color,
                IsActive = entity.IsActive,
                ProductCount = 0 // Will be calculated separately if needed
            };
        }

        /// <summary>
        /// Convert CategoryModel to CategoryEntity
        /// </summary>
        public static CategoryEntity ToEntity(this CategoryModel model)
        {
            return new CategoryEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Icon = model.Icon,
                Color = model.Color,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };
        }

        /// <summary>
        /// Update CategoryEntity from CategoryModel
        /// </summary>
        public static void UpdateFromModel(this CategoryEntity entity, CategoryModel model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Icon = model.Icon;
            entity.Color = model.Color;
            entity.IsActive = model.IsActive;
            entity.LastUpdated = DateTime.Now;
        }
    }
}