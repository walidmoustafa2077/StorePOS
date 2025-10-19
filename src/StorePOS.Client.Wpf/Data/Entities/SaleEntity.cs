using StorePOS.Client.Wpf.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorePOS.Client.Wpf.Data.Entities
{
    /// <summary>
    /// Sale entity for SQLite database storage
    /// </summary>
    [Table("Sales")]
    public class SaleEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string SaleNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalTax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ChangeAmount { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [MaxLength(100)]
        public string CashierName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign key for Shift relationship
        public int? ShiftId { get; set; }

        // Refund tracking
        public bool IsRefunded { get; set; } = false;
        public bool IsPartiallyRefunded { get; set; } = false;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRefundAmount { get; set; } = 0;

        public DateTime? RefundDate { get; set; }

        [MaxLength(500)]
        public string RefundReason { get; set; } = string.Empty;

        [MaxLength(100)]
        public string RefundedBy { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<SaleItemEntity> Items { get; set; } = new List<SaleItemEntity>();
        public virtual ShiftEntity? Shift { get; set; }
    }
}