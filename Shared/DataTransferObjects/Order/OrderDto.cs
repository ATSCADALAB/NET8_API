
using Shared.DataTransferObjects.Distributor;
using Shared.DataTransferObjects.User;

namespace Shared.DataTransferObjects.Order
{
    public record OrderDto
    {
        public Guid Id { get; init; }
        public string OrderCode { get; init; }
        public DateTime ExportDate { get; init; }
        public string VehicleNumber { get; init; }
        public int DriverNumber { get; init; }
        public string DriverName { get; init; }
        public string DriverPhoneNumber { get; init; }
        public int Status { get; init; }
        public int DistributorId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }

        // Thông tin liên quan
        public DistributorDto Distributor { get; init; }
        //public UserDto User { get; init; }
    }
}