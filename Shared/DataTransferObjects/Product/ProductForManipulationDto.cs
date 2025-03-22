namespace Shared.DataTransferObjects.Product
{
    public abstract record ProductForManipulationDto
    {
        public string TagID { get; init; }
        public int? OrderDetailId { get; init; } // Nullable để khớp với model
        public DateTime ShipmentDate { get; init; }
        public DateTime ManufactureDate { get; init; }
        public int DistributorId { get; init; }
        public int ProductInformationId { get; init; } // Thêm ProductInformationId
        public bool IsActive { get; init; } = true;
    }
}