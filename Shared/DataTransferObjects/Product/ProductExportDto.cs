using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Product
{
    public class ProductExportDto
    {
        public string TagID { get; set; }
        public string DistributorName { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string? GroupedPeriod { get; set; }
    }

}
