using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.InboundRecord
{
    public class InventoryReportDto
    {
        public string ProductName { get; set; }
        public int OpeningStockUnits { get; set; }
        public decimal OpeningStockWeight { get; set; }
        public int InQuantityUnits { get; set; }
        public decimal InQuantityWeight { get; set; }
        public int OutQuantityUnits { get; set; }
        public decimal OutQuantityWeight { get; set; }
        public int ClosingStockUnits { get; set; }
        public decimal ClosingStockWeight { get; set; }
    }
}
