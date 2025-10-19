using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service interface for shift business operations
    /// </summary>
    public interface IShiftService
    {
        /// <summary>
        /// Event raised when a shift is started or ended
        /// </summary>
        event EventHandler<ShiftModel>? ShiftChanged;

        /// <summary>
        /// Get all shifts
        /// </summary>
        Task<IEnumerable<ShiftModel>> GetAllShiftsAsync();

        /// <summary>
        /// Get shift by ID
        /// </summary>
        Task<ShiftModel?> GetShiftByIdAsync(int id);

        /// <summary>
        /// Get current active shift
        /// </summary>
        Task<ShiftModel?> GetCurrentShiftAsync();

        /// <summary>
        /// Start a new shift
        /// </summary>
        Task<ServiceResult<ShiftModel>> StartShiftAsync(decimal openingCash, string cashierName);

        /// <summary>
        /// End current shift
        /// </summary>
        Task<ServiceResult<ShiftModel>> EndShiftAsync(decimal closingCash, string notes = "");

        /// <summary>
        /// Check if there's an active shift
        /// </summary>
        Task<bool> HasActiveShiftAsync();

        /// <summary>
        /// Get shift summary with sales data
        /// </summary>
        Task<ShiftSummaryModel> GetShiftSummaryAsync(int shiftId);

        /// <summary>
        /// Get shifts by date range
        /// </summary>
        Task<IEnumerable<ShiftModel>> GetShiftsByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get shifts by cashier
        /// </summary>
        Task<IEnumerable<ShiftModel>> GetShiftsByCashierAsync(string cashierName);

        /// <summary>
        /// Validate shift operations
        /// </summary>
        Task<ServiceResult<bool>> ValidateShiftOperationsAsync();
    }
}