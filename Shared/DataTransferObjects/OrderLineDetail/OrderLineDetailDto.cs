using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.DataTransferObjects.Order;

namespace Shared.DataTransferObjects
{
    public record OrderLineDetailDto
    {
        public long Id { get; set; } // Long ID, tự động tăng
        public int SequenceNumber { get; set; } // Số thứ tự
        public Guid OrderId { get; set; } // OrderID
        public int Line { get; set; } // Line
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo
        public OrderDto Order { get; set; } // Navigation property
    }
}