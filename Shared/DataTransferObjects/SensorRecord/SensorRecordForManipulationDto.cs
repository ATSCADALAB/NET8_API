namespace Shared.DataTransferObjects.SensorRecord
{
    public abstract record SensorRecordForManipulationDto
    {
        public Guid OrderId { get; init; }
        public int OrderDetailId { get; init; }
        public int LineId { get; init; }
        public int SensorUnits { get; init; }
        public decimal SensorWeight { get; init; }
        public DateTime RecordTime { get; init; }
        public int Status { get; init; }
    }
}