namespace Shared.DataTransferObjects.OutboundRecord
{
    public abstract record OutboundRecordForManipulationDto
    {
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
        public DateTime OutboundDate { get; init; }
    }
}