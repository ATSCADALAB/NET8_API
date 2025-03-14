using Shared.DataTransferObjects.OrderDetail;
using Shared.DataTransferObjects.Distributor;

namespace Shared.DataTransferObjects.Product
{
    public record ProductDto
    {
        public int Id { get; init; }
        public string TagID { get; init; }
        public int OrderDetailId { get; init; }
        public DateTime ShipmentDate { get; init; }
        public DateTime ManufactureDate { get; init; }
        public int DistributorId { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        // Thông tin liên quan
        public OrderDetailDto OrderDetail { get; init; }
        public DistributorDto Distributor { get; init; }
    }
}