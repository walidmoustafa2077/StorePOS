using StorePOS.Domain.DTOs;
using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Extensions
{
    public static class SaleMappingExtensions
    {
        // EF-translatable projection for Sale
        public static readonly Expression<Func<Sale, SaleReadDto>> ToReadDtoExpr =
            s => new SaleReadDto(
                s.Id,
                s.CreatedAt,
                s.Subtotal,
                s.Discount,
                s.Tax,
                s.Total,
                s.PaidAmount,
                s.PaymentMethod.ToString(),
                s.Notes,
                s.Status.ToString(),
                s.Carts.Select(c => new SaleCartReadDto(
                    c.Id,
                    c.ProductId,
                    c.Product.Name,
                    c.Product.Sku,
                    c.Qty,
                    c.UnitPrice,
                    c.LineTotal
                )).ToList()
            );

        // Use this inside IQueryable pipelines so translation happens server-side
        public static IQueryable<SaleReadDto> SelectReadDto(this IQueryable<Sale> query) =>
            query.Select(ToReadDtoExpr);

        // Use these only after materialization (in-memory)
        public static SaleReadDto ToReadDto(this Sale s) =>
            ToReadDtoExpr.Compile().Invoke(s);
    }
}
