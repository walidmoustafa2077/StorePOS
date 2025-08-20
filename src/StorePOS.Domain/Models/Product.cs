using StorePOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace StorePOS.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = default!;
        public string Barcode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public int StockQty { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? UpdatedAt { get; set; }
        [Timestamp] public byte[]? RowVersion { get; set; }
    }
}
