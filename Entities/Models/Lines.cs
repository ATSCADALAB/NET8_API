using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("Lines")]
    [Index(nameof(LineNumber), IsUnique = true)] // Đảm bảo LineNumber duy nhất
    public class Line
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LineNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LineName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với OrderLineDetail (one-to-many)
        public virtual ICollection<OrderLineDetail> OrderLineDetails { get; set; } = new List<OrderLineDetail>();

        // Mối quan hệ với SensorRecord (one-to-many)
        public virtual ICollection<SensorRecord> SensorRecords { get; set; } = new List<SensorRecord>();
    }
}