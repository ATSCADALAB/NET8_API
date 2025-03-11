namespace Shared.DataTransferObjects
{
    public abstract record DistributorForManipulationDto
    {
        public string DistributorCode { get; init; } // Thêm DistributorCode

        public string DistributorName { get; init; } // Thay thế FirstName và LastName

        public string Address { get; init; } // Giữ nguyên

        public string PhoneNumber { get; init; } // Thêm PhoneNumber

        public string? ContactSource { get; init; } // Thêm ContactSource (có thể null)

        public string Area { get; init; } // Thêm Area (thay thế Country)

        public string? Note { get; init; } // Thêm Note (có thể null)

        public bool IsActive { get; init; } = true; // Thêm IsActive, mặc định là true
    }
}