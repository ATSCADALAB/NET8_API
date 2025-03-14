using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.ProductInformation;

namespace Shared.DataTransferObjects.OrderDetail
{
    public record OrderDetailDto
    {
        public int Id { get; init; }
        public Guid OrderId { get; init; }
        public int ProductInformationId { get; init; }
        public int RequestedUnits { get; init; }
        public decimal RequestedWeight { get; init; }
        public DateTime ManufactureDate { get; init; }
        public int DefectiveUnits { get; init; }
        public decimal DefectiveWeight { get; init; }
        public int ReplacedUnits { get; init; }
        public decimal ReplacedWeight { get; init; }
        public DateTime CreatedAt { get; init; }

        // Thông tin liên quan
        public OrderDto Order { get; init; }
        public ProductInformationDto ProductInformation { get; init; }
    }
}