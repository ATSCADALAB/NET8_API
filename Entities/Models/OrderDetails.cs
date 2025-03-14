using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public int ProductInformationId { get; set; }

        [Required]
        public int RequestedUnits { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RequestedWeight { get; set; } = 0m;

        public DateTime ManufactureDate { get; set; }

        public int DefectiveUnits { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal DefectiveWeight { get; set; } = 0m;

        public int ReplacedUnits { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal ReplacedWeight { get; set; } = 0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Order (many-to-one)
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        // Mối quan hệ với ProductInformation (many-to-one)
        [ForeignKey(nameof(ProductInformationId))]
        public virtual ProductInformation ProductInformation { get; set; }

        // Mối quan hệ với Product (one-to-many)
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Mối quan hệ với SensorRecord (one-to-many)
        public virtual ICollection<SensorRecord> SensorRecords { get; set; } = new List<SensorRecord>();
    }
}