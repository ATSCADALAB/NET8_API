using Shared.DataTransferObjects.OrderDetail;
using System;

namespace Shared.DataTransferObjects.Order
{
    public record OrderWithDetailsDto
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
        public string DistributorName { get; init; }
        public string Area { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public OrderDetailWithProductDto OrderDetail { get; init; } // Chỉ 1 OrderDetail thay vì danh sách
    }
}