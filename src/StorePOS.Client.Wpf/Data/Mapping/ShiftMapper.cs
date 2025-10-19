using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Data.Mapping
{
    /// <summary>
    /// Mapper for converting between ShiftEntity and ShiftModel
    /// </summary>
    public static class ShiftMapper
    {
        /// <summary>
        /// Convert ShiftEntity to ShiftModel
        /// </summary>
        public static ShiftModel ToModel(ShiftEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var model = new ShiftModel
            {
                Id = entity.Id,
                ShiftNumber = entity.ShiftNumber,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                StartingBalance = entity.InitialCashAmount,
                CurrentBalance = entity.ExpectedCashAmount,
                IsActive = entity.Status == ShiftStatus.Active,
                ShiftUser = entity.CashierName,
                Notes = entity.Notes ?? string.Empty
            };

            // If shift has sales, calculate the transactions
            if (entity.Sales?.Any() == true)
            {
                foreach (var sale in entity.Sales)
                {
                    var transaction = new TransactionModel
                    {
                        Id = sale.Id,
                        Amount = sale.TotalAmount,
                        Reference = sale.SaleNumber,
                        Timestamp = sale.SaleDate,
                        Type = TransactionType.Sale,
                        Description = $"Sale {sale.SaleNumber} - {sale.PaymentMethod}"
                    };
                    
                    model.AddTransaction(transaction);
                }
            }

            return model;
        }

        /// <summary>
        /// Convert ShiftModel to ShiftEntity
        /// </summary>
        public static ShiftEntity ToEntity(ShiftModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return new ShiftEntity
            {
                Id = model.Id,
                ShiftNumber = model.ShiftNumber,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                InitialCashAmount = model.StartingBalance,
                FinalCashAmount = model.IsActive ? null : model.CurrentBalance,
                ExpectedCashAmount = model.CurrentBalance,
                Status = model.IsActive ? ShiftStatus.Active : ShiftStatus.Closed,
                CashierName = model.ShiftUser,
                Notes = model.Notes,
                CreatedDate = model.StartTime,
                ModifiedDate = model.EndTime
            };
        }

        /// <summary>
        /// Update existing ShiftEntity from ShiftModel
        /// </summary>
        public static void UpdateEntity(ShiftEntity entity, ShiftModel model)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (model == null) throw new ArgumentNullException(nameof(model));

            entity.ShiftNumber = model.ShiftNumber;
            entity.EndTime = model.EndTime;
            entity.FinalCashAmount = model.IsActive ? null : model.CurrentBalance;
            entity.ExpectedCashAmount = model.CurrentBalance;
            entity.Status = model.IsActive ? ShiftStatus.Active : ShiftStatus.Closed;
            entity.CashierName = model.ShiftUser;
            entity.Notes = model.Notes;
            entity.ModifiedDate = DateTime.Now;
        }
    }
}