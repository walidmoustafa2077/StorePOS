using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    public interface ISaleService
    {
        Task<List<SaleReadDto>> SearchAsync(DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken ct = default);
        Task<SaleReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        
        Task<ServiceResult<SaleReadDto>> CreateAsync(SaleCreateDto dto, CancellationToken ct = default);
        Task<ServiceResult<SaleReadDto>> UpdateAsync(int id, SaleUpdateDto dto, CancellationToken ct = default);
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default);
        
        // Complete sale (reduce stock and mark as completed)
        Task<ServiceResult<SaleReadDto>> CompleteSaleAsync(int saleId, CancellationToken ct = default);
        
        // Cancel sale
        Task<ServiceResult<SaleReadDto>> CancelSaleAsync(int saleId, CancellationToken ct = default);
    }
}
