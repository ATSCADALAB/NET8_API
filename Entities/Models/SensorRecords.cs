using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("SensorRecords")]
    public class SensorRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public int OrderDetailId { get; set; }

        [Required]
        public int LineId { get; set; }

        [Required]
        public int SensorUnits { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SensorWeight { get; set; } = 0m;

        public DateTime RecordTime { get; set; } = DateTime.UtcNow;
        public int Status {  get; set; }

        // Mối quan hệ với Order (many-to-one)
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        // Mối quan hệ với OrderDetail (many-to-one)
        [ForeignKey(nameof(OrderDetailId))]
        public virtual OrderDetail OrderDetail { get; set; }

        // Mối quan hệ với Line (many-to-one)
        [ForeignKey(nameof(LineId))]
        public virtual Line Line { get; set; }
    }
}