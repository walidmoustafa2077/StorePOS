using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorePOS.Client.Wpf.Data.Entities
{
    /// <summary>
    /// Sale item entity for SQLite database storage
    /// </summary>
    [Table("SaleItems")]
    public class SaleItemEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SaleId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotalWithTax { get; set; }

        [MaxLength(255)]
        public string Notes { get; set; } = string.Empty;

        // Refund tracking for individual items
        public int QuantityRefunded { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; } = 0;

        public bool IsFullyRefunded { get; set; } = false;

        // Navigation properties
        [ForeignKey("SaleId")]
        public virtual SaleEntity Sale { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; } = null!;
    }
}