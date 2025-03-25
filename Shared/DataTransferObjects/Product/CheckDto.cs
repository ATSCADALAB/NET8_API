using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.Product
{
    public class CheckDto
    {
        public string TagID { get; set; }
        public CheckProductInformationDto ProductInformation { get; set; }
        public DateTime ProductDate { get; set; } // ManufactureDate
        public DateTime ShipmentDate { get; set; }
        public CheckDistributorDto Distributor { get; set; }
        public string Delivery { get; set; } // Giả định đây là trường tùy chỉnh
    }

    public class CheckProductInformationDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
    }

    public class CheckDistributorDto
    {
        public string DistributorName { get; set; }
        public string Area { get; set; } // AreaName từ bảng Area
    }
}