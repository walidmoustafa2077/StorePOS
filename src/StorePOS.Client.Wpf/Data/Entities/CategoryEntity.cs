using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorePOS.Client.Wpf.Data.Entities
{
    /// <summary>
    /// Category entity for SQLite database storage
    /// </summary>
    [Table("Categories")]
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Icon { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}