using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("OrderLineDetails")]
    public class OrderLineDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public int SequenceNumber { get; set; }

        [Required]
        public int LineId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; } // Nullable vì có thể dở dang

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Mối quan hệ với Order (many-to-one)
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        // Mối quan hệ với Line (many-to-one)
        [ForeignKey(nameof(LineId))]
        public virtual Line Line { get; set; }
    }
}