namespace Shared.DataTransferObjects.ProductInformation
{
    public record ProductInformationDto
    {
        public int Id { get; init; }
        public string ProductCode { get; init; }
        public string ProductName { get; init; }
        public string Unit { get; init; }
        public decimal WeightPerUnit { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}