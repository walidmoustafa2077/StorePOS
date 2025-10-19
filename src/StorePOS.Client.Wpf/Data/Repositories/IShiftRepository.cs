using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Shift entity operations
    /// </summary>
    public interface IShiftRepository : IRepository<ShiftEntity>
    {
        /// <summary>
        /// Get the current active shift
        /// </summary>
        /// <returns>Active shift or null if no active shift exists</returns>
        Task<ShiftEntity?> GetActiveShiftAsync();

        /// <summary>
        /// Get shifts for a specific date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Collection of shifts in the date range</returns>
        Task<IEnumerable<ShiftEntity>> GetShiftsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get shift with its sales included
        /// </summary>
        /// <param name="shiftId">Shift ID</param>
        /// <returns>Shift with sales or null if not found</returns>
        Task<ShiftEntity?> GetShiftWithSalesAsync(int shiftId);

        /// <summary>
        /// Check if there's an active shift
        /// </summary>
        /// <returns>True if there's an active shift, false otherwise</returns>
        Task<bool> HasActiveShiftAsync();

        /// <summary>
        /// Get the last shift (most recent by start time)
        /// </summary>
        /// <returns>Last shift or null if no shifts exist</returns>
        Task<ShiftEntity?> GetLastShiftAsync();
    }
}