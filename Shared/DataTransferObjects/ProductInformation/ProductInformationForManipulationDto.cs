namespace Shared.DataTransferObjects.ProductInformation
{
    public abstract record ProductInformationForManipulationDto
    {
        public string ProductCode { get; init; }
        public string ProductName { get; init; }
        public string Unit { get; init; }
        public decimal WeightPerUnit { get; init; }
        public bool IsActive { get; init; } = true;
    }
}