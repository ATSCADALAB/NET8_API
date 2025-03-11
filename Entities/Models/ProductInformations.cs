using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("ProductInformations")]
    [Index(nameof(ProductCode), IsUnique = true)]
    public class ProductInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

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
        public decimal Weight { get; set; } = 0m; // Đổi từ SpecificationInKg thành Weight

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Product (one-to-many)
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}