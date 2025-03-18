namespace Shared.DataTransferObjects.Report
{
    public class ProductDailyReportDto
    {
        public string Date { get; set; }
        public string LicensePlate { get; set; }
        public string LineName { get; set; }
        public string ProductName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSensorWeight { get; set; }
        public int TotalSensorUnits { get; set; }
        public decimal TotalWeightToDate { get; set; }
        public int TotalUnitsToDate { get; set; }
        public decimal DailyWeight { get; set; }
        public int DailyUnits { get; set; }
        public int RequestedUnits { get; set; } // Số sản phẩm yêu cầu (mục tiêu)
        public decimal RequestedWeight { get; set; } // Số kg yêu cầu (mục tiêu)
        public int ActualUnits { get; set; } // Số sản phẩm thực tế
        public decimal ActualWeight { get; set; } // Số kg thực tế
        public decimal CompletionPercentage { get; set; } // Phần trăm hoàn thành
    }
}