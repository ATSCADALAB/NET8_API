namespace Shared.DataTransferObjects.Stock
{
    public class StockReductionDto
    {
        public int ProductInformationId { get; set; }
        public int QuantityUnits { get; set; }
        public decimal QuantityWeight { get; set; }
    }
}