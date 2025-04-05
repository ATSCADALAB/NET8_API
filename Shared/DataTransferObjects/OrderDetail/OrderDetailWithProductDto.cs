using System;

namespace Shared.DataTransferObjects.OrderDetail
{
    public record OrderDetailWithProductDto
    {
        public int Id { get; init; }
        public Guid OrderId { get; init; }
        public int ProductInformationId { get; init; }
        public string ProductCode { get; init; }
        public string ProductName { get; init; }
        public int RequestedUnits { get; init; }
        public decimal RequestedWeight { get; init; }
        public DateTime? ManufactureDate { get; init; }
        public int DefectiveUnits { get; init; }
        public decimal DefectiveWeight { get; init; }
        public int ReplacedUnits { get; init; }
        public decimal ReplacedWeight { get; init; }
        public DateTime CreatedAt { get; init; }
        //public string? CreatedByName {  get; init; }
        //public string? UpdatedByName {  get; init; }
    }
}