using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Report
{
    public class RegionProductionReportDto
    {
        public string AreaName { get; set; }
        public string ProductName { get; set; }
        public string ExportPeriod { get; set; }
        public int TotalUnits { get; set; } // Sản lượng tháng
        public decimal TotalWeight { get; set; } // Trọng lượng tháng
        public int CumulativeUnits { get; set; } // Luỹ kế số lượng
        public decimal CumulativeWeight { get; set; } // Luỹ kế trọng lượng
    }
}
