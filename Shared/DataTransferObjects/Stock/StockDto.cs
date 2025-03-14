using Shared.DataTransferObjects.ProductInformation;

namespace Shared.DataTransferObjects.Stock
{
    public record StockDto
    {
        public int Id { get; init; }
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
        public DateTime LastUpdated { get; init; }

        // Thông tin liên quan
        public ProductInformationDto ProductInformation { get; init; }
    }
}