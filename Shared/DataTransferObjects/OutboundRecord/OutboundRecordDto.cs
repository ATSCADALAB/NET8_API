using Shared.DataTransferObjects.ProductInformation;

namespace Shared.DataTransferObjects.OutboundRecord
{
    public record OutboundRecordDto
    {
        public int Id { get; init; }
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
        public DateTime OutboundDate { get; init; }
        public DateTime CreatedAt { get; init; }

        // Thông tin liên quan
        public ProductInformationDto ProductInformation { get; init; }
    }
}