using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("InboundRecords")]
    public class InboundRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductInformationId { get; set; }

        [Required]
        public int QuantityUnits { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal QuantityWeight { get; set; } = 0m;

        public DateTime InboundDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với ProductInformation (many-to-one)
        [ForeignKey(nameof(ProductInformationId))]
        public virtual ProductInformation ProductInformation { get; set; }
    }
}