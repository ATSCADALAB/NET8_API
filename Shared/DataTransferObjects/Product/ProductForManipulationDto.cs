namespace Shared.DataTransferObjects.Product
{
    public abstract record ProductForManipulationDto
    {
        public string TagID { get; init; }

        public DateTime ShipmentDate { get; init; }

        public DateTime ProductDate { get; init; }

        public string Delivery { get; init; }

        public string StockOut { get; init; }

        public bool IsActive { get; init; } = true; // Mặc định là true, khớp với model

        public long DistributorId { get; init; } // Khóa ngoại

        public long ProductInformationId { get; init; } // Khóa ngoại
    }
}