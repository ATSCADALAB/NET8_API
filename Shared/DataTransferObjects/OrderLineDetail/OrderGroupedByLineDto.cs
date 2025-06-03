using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.Line;

namespace Shared.DataTransferObjects.OrderLineDetail
{
    public class OrderGroupedByLineDto
    {
        public int LineNumber { get; set; }
        public string LineName { get; set; }
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public string ProductName { get; set; }
        public int RequestedUnits { get; set; }
        public string DistributorName { get; set; }
    }
}