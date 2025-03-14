namespace Shared.DataTransferObjects.InboundRecord
{
    public abstract record InboundRecordForManipulationDto
    {
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
        public DateTime InboundDate { get; init; }
    }
}