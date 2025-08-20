using StorePOS.Domain.Enums;

namespace StorePOS.Domain.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? Notes { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Pending;
        public List<SaleCart> Carts { get; set; } = new();
    }
}
