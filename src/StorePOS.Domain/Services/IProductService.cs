using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    public interface IProductService
    {
        Task<List<ProductReadDto>> SearchAsync(string? q, CancellationToken ct);
        Task<ProductReadDto?> GetByBarcodeAsync(string barcode, CancellationToken ct);

        Task<ServiceResult<ProductReadDto>> CreateAsync(ProductCreateDto dto, CancellationToken ct);
        Task<ServiceResult<ProductReadDto>> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);
        Task<ServiceResult<ProductReadDto>> UpdateStockAsync(int id, ProductStockUpdateDto dto, CancellationToken ct);
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct);
    }
}
