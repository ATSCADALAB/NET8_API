using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("Products")]
    [Index(nameof(TagID), IsUnique = true, Name = "IX_Products_TagID")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string TagID { get; set; }

        public DateTime ShipmentDate { get; set; } = DateTime.UtcNow;

        // Loại bỏ trường Weight vì đã có trong ProductInformations
        // [Required]
        // [Column(TypeName = "decimal(10,2)")]
        // public decimal Weight { get; set; } = 0m;

        public DateTime ProductDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Delivery { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string StockOut { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Distributor (many-to-one)
        public long DistributorId { get; set; }

        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor Distributor { get; set; }

        // Mối quan hệ với ProductInformations (many-to-one)
        public long ProductInformationId { get; set; }

        [ForeignKey(nameof(ProductInformationId))]
        public virtual ProductInformation ProductInformation { get; set; }
    }
}