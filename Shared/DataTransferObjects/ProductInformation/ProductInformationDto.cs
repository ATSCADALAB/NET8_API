namespace Shared.DataTransferObjects.ProductInformation
{
    public record ProductInformationDto
    {
        public long Id { get; init; } // Khớp với model, dùng long thay vì Guid

        public string ProductCode { get; init; }

        public string ProductName { get; init; }

        public string Unit { get; init; }

        public decimal Weight { get; init; }

        public bool IsActive { get; init; } // Thay thế status
    }
}