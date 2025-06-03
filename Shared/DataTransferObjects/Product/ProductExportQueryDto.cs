using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Product
{
    public class ProductExportQueryDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? DistributorId { get; set; }
        public int? ProductInformationId { get; set; }
        public string GroupBy { get; set; } = "none"; // none, day, week, month
    }

}
