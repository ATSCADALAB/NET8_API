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

        // Thông tin liên quan
        public OrderDto Order { get; init; }
        public OrderDetailDto OrderDetail { get; init; }
        public LineDto Line { get; init; }
    }
}