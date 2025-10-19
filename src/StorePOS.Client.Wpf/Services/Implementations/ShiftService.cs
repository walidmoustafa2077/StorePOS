using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data.Mapping;
using StorePOS.Client.Wpf.MVVM.Models;
using Microsoft.Extensions.Logging;
using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Implementation of IShiftService providing shift business operations
    /// </summary>
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<ShiftService> _logger;

        /// <summary>
        /// Event raised when a shift is started or ended
        /// </summary>
        public event EventHandler<ShiftModel>? ShiftChanged;

        public ShiftService(
            IShiftRepository shiftRepository,
            ISaleRepository saleRepository,
            ILogger<ShiftService> logger)
        {
            _shiftRepository = shiftRepository;
            _saleRepository = saleRepository;
            _logger = logger;
        }

        /// <summary>
        /// Raises the ShiftChanged event to notify subscribers of shift changes
        /// </summary>
        private void OnShiftChanged(ShiftModel shift)
        {
            ShiftChanged?.Invoke(this, shift);
        }

        public async Task<IEnumerable<ShiftModel>> GetAllShiftsAsync()
        {
            try
            {
                var entities = await _shiftRepository.GetAllAsync();
                return entities.Select(e => ShiftMapper.ToModel(e));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all shifts");
                throw;
            }
        }

        public async Task<ShiftModel?> GetShiftByIdAsync(int id)
        {
            try
            {
                var entity = await _shiftRepository.GetByIdAsync(id);
                return entity != null ? ShiftMapper.ToModel(entity) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shift by ID: {ShiftId}", id);
                throw;
            }
        }

        public async Task<ShiftModel?> GetCurrentShiftAsync()
        {
            try
            {
                var entities = await _shiftRepository.FindAsync(s => s.Status == ShiftStatus.Active);
                var currentShift = entities.FirstOrDefault();
                return currentShift != null ? ShiftMapper.ToModel(currentShift) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting current active shift");
                throw;
            }
        }

        public async Task<ServiceResult<ShiftModel>> StartShiftAsync(decimal openingCash, string cashierName)
        {
            try
            {
                // Validate inputs
                var validationResult = ValidateShiftStart(openingCash, cashierName);
                if (!validationResult.IsSuccess)
                {
                    return ServiceResult<ShiftModel>.ValidationFailure(validationResult.ValidationErrors);
                }

                // Check if there's already an active shift
                if (await HasActiveShiftAsync())
                {
                    return ServiceResult<ShiftModel>.ValidationFailure("There is already an active shift. Please end the current shift before starting a new one.");
                }

                // Create new shift
                var shift = new ShiftModel
                {
                    StartTime = DateTime.Now,
                    StartingBalance = openingCash,
                    ShiftUser = cashierName,
                    IsActive = true,
                    Notes = string.Empty
                };

                var entity = ShiftMapper.ToEntity(shift);
                var createdEntity = await _shiftRepository.AddAsync(entity);
                var createdModel = ShiftMapper.ToModel(createdEntity);

                _logger.LogInformation("Shift started successfully: ID {ShiftId}, Cashier: {CashierName}, Opening Cash: {OpeningCash}", 
                    createdModel.Id, cashierName, openingCash);

                // Raise ShiftChanged event to notify subscribers
                OnShiftChanged(createdModel);

                return ServiceResult<ShiftModel>.Success(createdModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while starting shift for cashier: {CashierName}", cashierName);
                return ServiceResult<ShiftModel>.Failure(ex);
            }
        }

        public async Task<ServiceResult<ShiftModel>> EndShiftAsync(decimal closingCash, string notes = "")
        {
            try
            {
                // Get current active shift
                var currentShift = await GetCurrentShiftAsync();
                if (currentShift == null)
                {
                    return ServiceResult<ShiftModel>.ValidationFailure("No active shift found to end");
                }

                // Validate closing cash
                if (closingCash < 0)
                {
                    return ServiceResult<ShiftModel>.ValidationFailure("Closing cash cannot be negative");
                }

                // Update shift
                currentShift.EndTime = DateTime.Now;
                currentShift.CurrentBalance = closingCash;
                currentShift.IsActive = false;
                currentShift.Notes = notes;

                var entity = await _shiftRepository.GetByIdAsync(currentShift.Id);
                if (entity == null)
                {
                    return ServiceResult<ShiftModel>.Failure("Shift entity not found");
                }

                ShiftMapper.UpdateEntity(entity, currentShift);
                var updatedEntity = await _shiftRepository.UpdateAsync(entity);
                var updatedModel = ShiftMapper.ToModel(updatedEntity);

                _logger.LogInformation("Shift ended successfully: ID {ShiftId}, Cashier: {CashierName}, Closing Cash: {ClosingCash}", 
                    updatedModel.Id, updatedModel.CashierName, closingCash);

                // Raise ShiftChanged event to notify subscribers
                OnShiftChanged(updatedModel);

                return ServiceResult<ShiftModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while ending shift with closing cash: {ClosingCash}", closingCash);
                return ServiceResult<ShiftModel>.Failure(ex);
            }
        }

        public async Task<bool> HasActiveShiftAsync()
        {
            try
            {
                var activeShifts = await _shiftRepository.FindAsync(s => s.Status == ShiftStatus.Active);
                return activeShifts.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for active shift");
                throw;
            }
        }

        public async Task<ShiftSummaryModel> GetShiftSummaryAsync(int shiftId)
        {
            try
            {
                var shift = await GetShiftByIdAsync(shiftId);
                if (shift == null)
                    throw new InvalidOperationException("Shift not found");

                // Get sales for this shift
                var sales = await _saleRepository.FindAsync(s => s.ShiftId == shiftId);
                var salesList = sales.ToList();

                var summary = new ShiftSummaryModel
                {
                    ShiftId = shift.Id,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    CashierName = shift.CashierName,
                    OpeningCash = shift.StartingBalance,
                    ClosingCash = shift.CurrentBalance,
                    TotalSales = salesList.Count,
                    TotalRevenue = salesList.Sum(s => s.TotalAmount),
                    IsActive = shift.IsActive,
                    Notes = shift.Notes
                };

                // Calculate cash variance
                var expectedCash = shift.StartingBalance + summary.TotalRevenue;
                summary.CashVariance = shift.CurrentBalance - expectedCash;

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shift summary for shift: {ShiftId}", shiftId);
                throw;
            }
        }

        public async Task<IEnumerable<ShiftModel>> GetShiftsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var entities = await _shiftRepository.FindAsync(s => 
                    s.StartTime >= fromDate && s.StartTime <= toDate);
                return entities.Select(e => ShiftMapper.ToModel(e)).OrderByDescending(s => s.StartTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shifts by date range: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<ShiftModel>> GetShiftsByCashierAsync(string cashierName)
        {
            try
            {
                var entities = await _shiftRepository.FindAsync(s => s.CashierName == cashierName);
                return entities.Select(e => ShiftMapper.ToModel(e)).OrderByDescending(s => s.StartTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting shifts by cashier: {CashierName}", cashierName);
                throw;
            }
        }

        public async Task<ServiceResult<bool>> ValidateShiftOperationsAsync()
        {
            var errors = new List<string>();

            try
            {
                // Check for active shift
                var hasActiveShift = await HasActiveShiftAsync();
                if (!hasActiveShift)
                {
                    errors.Add("No active shift found. Sales operations require an active shift.");
                }

                // Check for any shifts that have been open too long (e.g., more than 24 hours)
                var currentShift = await GetCurrentShiftAsync();
                if (currentShift != null && currentShift.IsActive)
                {
                    var shiftDuration = DateTime.Now - currentShift.StartTime;
                    if (shiftDuration.TotalHours > 24)
                    {
                        errors.Add($"Current shift has been active for {shiftDuration.TotalHours:F1} hours. Consider ending the shift.");
                    }
                }

                if (errors.Any())
                {
                    return ServiceResult<bool>.ValidationFailure(errors);
                }

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating shift operations");
                return ServiceResult<bool>.Failure(ex);
            }
        }

        private ServiceResult<bool> ValidateShiftStart(decimal openingCash, string cashierName)
        {
            var errors = new List<string>();

            if (openingCash < 0)
                errors.Add("Opening cash cannot be negative");

            if (string.IsNullOrWhiteSpace(cashierName))
                errors.Add("Cashier name is required");
            else if (cashierName.Length > 100)
                errors.Add("Cashier name cannot exceed 100 characters");

            if (errors.Any())
            {
                return ServiceResult<bool>.ValidationFailure(errors);
            }

            return ServiceResult<bool>.Success(true);
        }
    }
}