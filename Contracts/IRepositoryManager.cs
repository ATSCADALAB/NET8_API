using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IAreaRepository Area { get; } // Thêm cho Areas
        ILineRepository Line { get; } // Thêm cho Lines
        IDistributorRepository Distributor { get; } // Đã có trong yêu cầu của bạn
        IProductInformationRepository ProductInformation { get; } // Đã có trong yêu cầu của bạn
        IOrderRepository Order { get; } // Đã có trong yêu cầu của bạn
        IOrderDetailRepository OrderDetail { get; } // Thêm cho OrderDetails
        IOrderLineDetailRepository OrderLineDetail { get; } // Đã có trong yêu cầu của bạn
        ISensorRecordRepository SensorRecord { get; } // Thêm cho SensorRecords
        IProductRepository Product { get; } // Đã có trong yêu cầu của bạn
        IStockRepository Stock { get; } // Thêm cho Stock
        IInboundRecordRepository InboundRecord { get; } // Thêm cho InboundRecords
        IOutboundRecordRepository OutboundRecord { get; } // Thêm cho InboundRecords

        IAuditRepository Audit { get; }
        ICategoryRepository Category { get; }
        IPermissionRepository Permission { get; }
        IRolePermissionRepository RolePermission { get; }

        Task SaveAsync();
    }
}