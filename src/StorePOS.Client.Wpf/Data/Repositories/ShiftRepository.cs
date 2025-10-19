using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Shift entity operations
    /// </summary>
    public class ShiftRepository : Repository<ShiftEntity>, IShiftRepository
    {
        public ShiftRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ShiftEntity?> GetActiveShiftAsync()
        {
            return await _dbSet
                .Where(s => s.Status == ShiftStatus.Active)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShiftEntity>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<ShiftEntity?> GetShiftWithSalesAsync(int shiftId)
        {
            return await _dbSet
                .Include(s => s.Sales)
                .ThenInclude(sale => sale.Items)
                .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(s => s.Id == shiftId);
        }

        public async Task<bool> HasActiveShiftAsync()
        {
            return await _dbSet.AnyAsync(s => s.Status == ShiftStatus.Active);
        }

        public async Task<ShiftEntity?> GetLastShiftAsync()
        {
            return await _dbSet
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<ShiftEntity>> GetAllAsync()
        {
            return await _dbSet
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();
        }

        public override async Task<ShiftEntity?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Sales)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}