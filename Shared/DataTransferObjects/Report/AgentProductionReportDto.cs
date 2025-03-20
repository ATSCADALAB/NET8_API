namespace Shared.DataTransferObjects.Report
{
    public class AgentProductionReportDto
    {
        public string DistributorName { get; set; }
        public string ProductName { get; set; }
        public string ExportPeriod { get; set; } // Định dạng MM/YYYY
        public int MonthlyUnits { get; set; } // Số lượng trong tháng
        public int CumulativeUnits { get; set; } // Số lượng luỹ kế
        public decimal MonthlyWeight { get; set; } // Trọng lượng trong tháng
        public decimal CumulativeWeight { get; set; } // Trọng lượng luỹ kế
    }
}