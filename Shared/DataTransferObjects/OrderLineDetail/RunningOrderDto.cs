using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.OrderLineDetail
{
    public class RunningOrderDto
    {
        public int LineId { get; set; }
        public int LineNumber { get; set; }
        public string LineName { get; set; }
        public Guid OrderId { get; set; } // Sử dụng string vì OrderId là CHAR(36) trong MySQL
        public string OrderCode { get; set; }
        public int DistributorId { get; set; }
        public string DistributorName { get; set; }
        public int ProductInformationId { get; set; }
        public string ProductName { get; set; }
        public int RequestedUnits { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime ExportDate { get; set; } // Thêm ngày xuất hàng
        public DateTime RecordTime { get; set; } // Thêm RecordTime (StartTime từ OrderLineDetails)
    }
}
