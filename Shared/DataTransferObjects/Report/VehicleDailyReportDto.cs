public class VehicleDailyReportDto
{
    public string Date { get; set; }
    public string VehicleNumber { get; set; }  // Biển số xe
    public string ProductName { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSensorWeight { get; set; }
    public int TotalSensorUnits { get; set; }
}