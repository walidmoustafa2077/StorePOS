using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Data.Mapping
{
    /// <summary>
    /// Extension methods for mapping between ProductEntity and ProductModel
    /// </summary>
    public static class ProductMappingExtensions
    {
        /// <summary>
        /// Convert ProductEntity to ProductModel
        /// </summary>
        public static ProductModel ToModel(this ProductEntity entity)
        {
            return new ProductModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Cost = entity.Cost,
                StockQuantity = entity.StockQuantity,
                Category = entity.Category,
                SKU = entity.SKU,
                Barcode = entity.Barcode,
                ImageUrl = entity.ImageUrl,
                CreatedDate = entity.CreatedDate,
                LastUpdated = entity.LastUpdated,
                Brand = entity.Brand,
                Supplier = entity.Supplier,
                Weight = entity.Weight,
                Unit = entity.Unit,
                Discount = entity.Discount,
                IsTaxable = entity.IsTaxable,
                TaxRate = entity.TaxRate
            };
        }

        /// <summary>
        /// Convert ProductModel to ProductEntity
        /// </summary>
        public static ProductEntity ToEntity(this ProductModel model)
        {
            return new ProductEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Cost = model.Cost,
                StockQuantity = model.StockQuantity,
                Category = model.Category,
                SKU = model.SKU,
                Barcode = model.Barcode,
                ImageUrl = model.ImageUrl,
                IsActive = model.IsActive,
                CreatedDate = model.CreatedDate,
                LastUpdated = model.LastUpdated,
                Brand = model.Brand,
                Supplier = model.Supplier,
                Weight = model.Weight,
                Unit = model.Unit,
                Discount = model.Discount,
                IsTaxable = model.IsTaxable,
                TaxRate = model.TaxRate
            };
        }

        /// <summary>
        /// Update ProductEntity from ProductModel
        /// </summary>
        public static void UpdateFromModel(this ProductEntity entity, ProductModel model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.Cost = model.Cost;
            entity.StockQuantity = model.StockQuantity;
            entity.Category = model.Category;
            entity.SKU = model.SKU;
            entity.Barcode = model.Barcode;
            entity.ImageUrl = model.ImageUrl;
            entity.IsActive = model.IsActive;
            entity.LastUpdated = DateTime.Now;
            entity.Brand = model.Brand;
            entity.Supplier = model.Supplier;
            entity.Weight = model.Weight;
            entity.Unit = model.Unit;
            entity.Discount = model.Discount;
            entity.IsTaxable = model.IsTaxable;
            entity.TaxRate = model.TaxRate;
        }
    }
}