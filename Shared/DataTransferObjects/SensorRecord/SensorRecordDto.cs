using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.OrderDetail;
using Shared.DataTransferObjects.Line;

namespace Shared.DataTransferObjects.SensorRecord
{
    public record SensorRecordDto
    {
        public int Id { get; init; }
        public Guid OrderId { get; init; }
        public int OrderDetailId { get; init; }
        public int LineId { get; init; }
        public int SensorUnits { get; init; }
        public decimal SensorWeight { get; init; }
        public DateTime RecordTime { get; init; }
        public int Status {  get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }

        // Thông tin liên quan
        public OrderDto Order { get; init; }
        public OrderDetailDto OrderDetail { get; init; }
        public LineDto Line { get; init; }
    }
}