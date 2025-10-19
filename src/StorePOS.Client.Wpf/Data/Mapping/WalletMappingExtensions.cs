using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Data.Mapping
{
    /// <summary>
    /// Extension methods for mapping between WalletEntity and WalletModel
    /// </summary>
    public static class WalletMappingExtensions
    {
        /// <summary>
        /// Convert WalletEntity to WalletModel
        /// </summary>
        public static WalletModel ToModel(this WalletEntity entity)
        {
            return new WalletModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Balance = entity.Balance,
                IsActive = entity.IsActive
            };
        }

        /// <summary>
        /// Convert WalletModel to WalletEntity
        /// </summary>
        public static WalletEntity ToEntity(this WalletModel model)
        {
            return new WalletEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Balance = model.Balance,
                IsActive = model.IsActive,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };
        }

        /// <summary>
        /// Update entity from model
        /// </summary>
        public static WalletEntity UpdateFromModel(this WalletEntity entity, WalletModel model)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Balance = model.Balance;
            entity.IsActive = model.IsActive;
            entity.LastUpdated = DateTime.Now;
            return entity;
        }
    }
}