namespace Shared.DataTransferObjects.Report
{
    public class AgentProductionReportDto
    {
        public string DistributorName { get; set; }
        public string ProductName { get; set; }
        public string ExportPeriod { get; set; } // Định dạng MM/YYYY
        public int CumulativeUnits { get; set; }
        public decimal CumulativeWeight { get; set; }
    }
}