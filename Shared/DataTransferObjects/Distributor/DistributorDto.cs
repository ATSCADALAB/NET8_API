namespace Shared.DataTransferObjects
{
    public record DistributorDto
    {
        public long Id { get; init; } // Đổi từ Guid sang long để khớp với model

        public string DistributorCode { get; init; } // Thêm DistributorCode

        public string DistributorName { get; init; } // Thay thế FirstName và LastName

        public string Address { get; init; } // Giữ nguyên

        public string PhoneNumber { get; init; } // Thêm PhoneNumber

        public string? ContactSource { get; init; } // Thêm ContactSource (có thể null)

        public string Area { get; init; } // Thêm Area (thay thế Country)

        public string? Note { get; init; } // Thêm Note (có thể null)

        public bool IsActive { get; init; } // Thay thế status
    }
}