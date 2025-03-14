using Shared.DataTransferObjects.ProductInformation;

namespace Shared.DataTransferObjects.InboundRecord
{
    public record InboundRecordDto
    {
        public int Id { get; init; }
        public int ProductInformationId { get; init; }
        public int QuantityUnits { get; init; }
        public decimal QuantityWeight { get; init; }
        public DateTime InboundDate { get; init; }
        public DateTime CreatedAt { get; init; }

        // Thông tin liên quan
        public ProductInformationDto ProductInformation { get; init; }
    }
}