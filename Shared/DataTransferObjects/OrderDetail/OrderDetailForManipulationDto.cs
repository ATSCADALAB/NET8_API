namespace Shared.DataTransferObjects.OrderDetail
{
    public abstract record OrderDetailForManipulationDto
    {
        public Guid OrderId { get; init; }
        public int ProductInformationId { get; init; }
        public int RequestedUnits { get; init; }
        public decimal RequestedWeight { get; init; }
        public DateTime ManufactureDate { get; init; }
        public int DefectiveUnits { get; init; }
        public decimal DefectiveWeight { get; init; }
        public int ReplacedUnits { get; init; }
        public decimal ReplacedWeight { get; init; }
    }
}