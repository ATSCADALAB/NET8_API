namespace Shared.DataTransferObjects.Order
{
    public abstract record OrderForManipulationDto
    {
        public string OrderCode { get; init; }
        public DateTime ExportDate { get; init; }
        public string VehicleNumber { get; init; }
        public int DriverNumber { get; init; }
        public string DriverName { get; init; }
        public string DriverPhoneNumber { get; init; }
        public int Status { get; init; }
        public int DistributorId { get; init; }
    }
}