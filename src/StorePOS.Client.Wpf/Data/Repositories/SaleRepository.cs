using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Sale repository implementation
    /// </summary>
    public class SaleRepository : Repository<SaleEntity>, ISaleRepository
    {
        public SaleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SaleEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleEntity>> GetTodaysSalesAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            
            return await GetByDateRangeAsync(today, tomorrow);
        }

        public async Task<SaleEntity?> GetByNumberAsync(string saleNumber)
        {
            return await _dbSet
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber);
        }

        public async Task<SaleEntity?> GetWithItemsAsync(int saleId)
        {
            return await _dbSet
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == saleId);
        }

        public async Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<int> GetTotalSalesCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .CountAsync(s => s.SaleDate >= startDate && s.SaleDate <= endDate);
        }

        public async Task<string> GenerateNextSaleNumberAsync()
        {
            var today = DateTime.Today;
            var datePrefix = today.ToString("yyyyMMdd");
            
            var lastSaleNumber = await _dbSet
                .Where(s => s.SaleNumber.StartsWith(datePrefix))
                .OrderByDescending(s => s.SaleNumber)
                .Select(s => s.SaleNumber)
                .FirstOrDefaultAsync();

            if (lastSaleNumber == null)
            {
                return $"{datePrefix}001";
            }

            // Extract the sequence number and increment
            var sequencePart = lastSaleNumber.Substring(8);
            if (int.TryParse(sequencePart, out var sequence))
            {
                return $"{datePrefix}{(sequence + 1):D3}";
            }

            return $"{datePrefix}001";
        }

        public async Task<IEnumerable<SaleEntity>> GetByShiftIdAsync(int shiftId)
        {
            return await _dbSet
                .Where(s => s.ShiftId == shiftId)
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesAmountByShiftAsync(int shiftId)
        {
            return await _dbSet
                .Where(s => s.ShiftId == shiftId)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<int> GetTotalSalesCountByShiftAsync(int shiftId)
        {
            return await _dbSet
                .CountAsync(s => s.ShiftId == shiftId);
        }

        public async Task<SaleEntity> UpdateSaleWithItemsAsync(SaleEntity sale)
        {
            // Update the sale entity
            _context.Entry(sale).State = EntityState.Modified;

            // Update all sale items explicitly
            foreach (var item in sale.Items)
            {
                _context.Entry(item).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return sale;
        }
    }
}