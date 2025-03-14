namespace Shared.DataTransferObjects.Distributor
{
    public abstract record DistributorForManipulationDto
    {
        public string DistributorCode { get; init; }
        public string DistributorName { get; init; }
        public string Address { get; init; }
        public string ContactSource { get; init; }
        public string PhoneNumber { get; init; }
        public int AreaId { get; init; }
        public bool IsActive { get; init; } = true;
    }
}