using StorePOS.Client.Wpf.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorePOS.Client.Wpf.Data.Entities
{
    /// <summary>
    /// Shift entity for tracking work shifts and cash management
    /// </summary>
    [Table("Shifts")]
    public class ShiftEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ShiftNumber { get; set; } = string.Empty;

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime? EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialCashAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FinalCashAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpectedCashAmount { get; set; }

        public ShiftStatus Status { get; set; } = ShiftStatus.Active;

        [MaxLength(100)]
        public string CashierName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<SaleEntity> Sales { get; set; } = new List<SaleEntity>();
    }
}