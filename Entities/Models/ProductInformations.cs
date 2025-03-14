using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("ProductInformations")]
    [Index(nameof(ProductCode), IsUnique = true)] // Đảm bảo ProductCode duy nhất
    public class ProductInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ProductCode { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Unit { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal WeightPerUnit { get; set; } = 0m;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với OrderDetail (one-to-many)
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Mối quan hệ với Product (one-to-many)
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Mối quan hệ với Stock (one-to-one)
        public virtual Stock Stock { get; set; }

        // Mối quan hệ với InboundRecord (one-to-many)
        public virtual ICollection<InboundRecord> InboundRecords { get; set; } = new List<InboundRecord>();
    }
}