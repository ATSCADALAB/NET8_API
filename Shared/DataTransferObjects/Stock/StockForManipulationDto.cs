namespace Shared.DataTransferObjects.Stock
{
    public abstract record StockForManipulationDto
    {
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
    }
}