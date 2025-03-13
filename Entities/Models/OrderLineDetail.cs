using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Models
{
    [Table("OrderLineDetails")]
    public class OrderLineDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } // Long ID, tự động tăng

        [Required]
        public int SequenceNumber { get; set; } // Số thứ tự

        [Required]
        [StringLength(36)] // Độ dài phù hợp với UUID
        public Guid OrderId { get; set; } // OrderID

        [Required]
        [Range(1, 4)] // Giới hạn Line từ 1 đến 4
        public int Line { get; set; } // Line

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo

        // Mối quan hệ với Order (many-to-one)
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } // Navigation property
    }
}