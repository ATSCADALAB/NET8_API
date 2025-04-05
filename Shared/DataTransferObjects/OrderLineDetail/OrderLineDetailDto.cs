using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.Line;

namespace Shared.DataTransferObjects.OrderLineDetail
{
    public record OrderLineDetailDto
    {
        public int Id { get; init; }
        public Guid OrderId { get; init; }
        public int SequenceNumber { get; init; }
        public int LineId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime? EndTime { get; init; }
        public DateTime CreatedAt { get; init; }

        // Thông tin liên quan
        public OrderDto? Order { get; init; }
        public LineDto Line { get; init; }
    }
}