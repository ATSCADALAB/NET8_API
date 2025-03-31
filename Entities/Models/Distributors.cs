using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("Distributors")]
    [Index(nameof(DistributorCode), IsUnique = true)] // Đảm bảo DistributorCode duy nhất
    public class Distributor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string DistributorCode { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string DistributorName { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string ?ContactSource { get; set; }
        [StringLength(200, MinimumLength = 1)]
        public string? Address { get; set; }

        [StringLength(200, MinimumLength = 1)]
        public string ? Province { get; set; }

        [StringLength(20, MinimumLength = 10)]
        public string ?PhoneNumber { get; set; }

        [Required]
        public int AreaId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Area (many-to-one)
        [ForeignKey(nameof(AreaId))]
        public virtual Area Area { get; set; }

        // Mối quan hệ với Order (one-to-many)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Mối quan hệ với Product (one-to-many)
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}