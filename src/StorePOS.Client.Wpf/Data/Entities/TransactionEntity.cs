using StorePOS.Client.Wpf.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorePOS.Client.Wpf.Data.Entities
{
    /// <summary>
    /// Transaction entity for tracking all financial transactions (sales, cash drops, cash adds, etc.)
    /// </summary>
    [Table("Transactions")]
    public class TransactionEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Reference { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Foreign Keys
        public int? ShiftId { get; set; }
        
        [Required]
        public int WalletId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ShiftId))]
        public virtual ShiftEntity? Shift { get; set; }

        [ForeignKey(nameof(WalletId))]
        public virtual WalletEntity Wallet { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
