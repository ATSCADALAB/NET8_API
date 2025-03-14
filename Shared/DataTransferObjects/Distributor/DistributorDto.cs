using Shared.DataTransferObjects.Area;

namespace Shared.DataTransferObjects.Distributor
{
    public record DistributorDto
    {
        public int Id { get; init; }
        public string DistributorCode { get; init; }
        public string DistributorName { get; init; }
        public string Address { get; init; }
        public string ContactSource { get; set; }
        public string PhoneNumber { get; init; }
        public int AreaId { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }

        // Thông tin liên quan
        public AreaDto Area { get; init; }
    }
}