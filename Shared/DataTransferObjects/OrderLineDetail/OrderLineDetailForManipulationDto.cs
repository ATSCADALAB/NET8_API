namespace Shared.DataTransferObjects.OrderLineDetail
{
    public abstract record OrderLineDetailForManipulationDto
    {
        public Guid OrderId { get; init; }
        public int SequenceNumber { get; init; }
        public int LineId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime? EndTime { get; init; }
    }
}