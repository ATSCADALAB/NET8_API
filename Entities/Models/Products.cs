using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("Products")]
    [Index(nameof(TagID), IsUnique = true)] // Đảm bảo TagID duy nhất
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string TagID { get; set; }

        [StringLength(100)]
        public string? StockOut { get; set; }

        [StringLength(200)]
        public string? Distributor2 { get; set; }

        public int? OrderDetailId { get; set; } // Nullable vì OrderId có thể null

        public DateTime ShipmentDate { get; set; } = DateTime.UtcNow;

        public DateTime ManufactureDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int DistributorId { get; set; }

        [Required]
        public int ProductInformationId { get; set; } // Thêm trường ProductInformationId, bắt buộc

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(200)]
        public string? Delivery { get; set; } 

        // Mối quan hệ với OrderDetail (many-to-one, nullable)
        [ForeignKey(nameof(OrderDetailId))]
        public virtual OrderDetail OrderDetail { get; set; }

        // Mối quan hệ với Distributor (many-to-one)
        [ForeignKey(nameof(DistributorId))]
        public virtual Distributor Distributor { get; set; }

        // Mối quan hệ với ProductInformation (many-to-one)
        [ForeignKey(nameof(ProductInformationId))]
        public virtual ProductInformation ProductInformation { get; set; }
    }
}