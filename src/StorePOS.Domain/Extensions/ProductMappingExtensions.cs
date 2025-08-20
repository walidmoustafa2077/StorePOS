using StorePOS.Domain.DTOs;
using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Extensions
{
    public static class ProductMappingExtensions
    {
        // EF-translatable projection
        public static readonly Expression<Func<Product, ProductReadDto>> ToReadDtoExpr =
            p => new ProductReadDto(p.Id, p.Sku, p.Barcode, p.Name, p.Category!.Name, p.Price, p.StockQty);

        // Use this inside IQueryable pipelines so translation happens server-side
        public static IQueryable<ProductReadDto> SelectReadDto(this IQueryable<Product> query) =>
            query.Select(ToReadDtoExpr);

        // Use these only after materialization (in-memory)
        public static ProductReadDto ToReadDto(this Product p) =>
            ToReadDtoExpr.Compile().Invoke(p);

    }
}
