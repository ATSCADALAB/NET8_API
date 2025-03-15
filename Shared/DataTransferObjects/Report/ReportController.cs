using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Report
{
    public class ProductDailyReportDto
    {
        public string Date { get; set; }
        public string LineName { get; set; }
        public string ProductName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSensorWeight { get; set; }
        public int TotalSensorUnits { get; set; }
    }
}
