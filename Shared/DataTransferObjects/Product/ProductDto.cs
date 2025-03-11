using Shared.DataTransferObjects.ProductInformation;

namespace Shared.DataTransferObjects.Product
{
    public record ProductDto
    {
        public long Id { get; init; }
        public string TagID { get; init; }
        public DateTime ShipmentDate { get; init; }
        public DateTime ProductDate { get; init; }
        public string Delivery { get; init; }
        public string StockOut { get; init; }
        public bool IsActive { get; init; }
        public long DistributorId { get; init; }
        public long ProductInformationId { get; init; }

        // Thêm DTO con để hiển thị chi tiết
        public DistributorDto Distributor { get; init; }
        public ProductInformationDto ProductInformation { get; init; }
    }
}