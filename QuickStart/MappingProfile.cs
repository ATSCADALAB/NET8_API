using AutoMapper;
using Entities.Identity;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Area;
using Shared.DataTransferObjects.AuditLog;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Category;
using Shared.DataTransferObjects.Dashboard;
using Shared.DataTransferObjects.Distributor;
using Shared.DataTransferObjects.InboundRecord;
using Shared.DataTransferObjects.Line;
using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.OrderDetail;
using Shared.DataTransferObjects.OrderLineDetail;
using Shared.DataTransferObjects.OutboundRecord;
using Shared.DataTransferObjects.Permission;
using Shared.DataTransferObjects.Product;
using Shared.DataTransferObjects.ProductInformation;
using Shared.DataTransferObjects.RolePermission;
using Shared.DataTransferObjects.SensorRecord;
using Shared.DataTransferObjects.Stock;
using Shared.DataTransferObjects.User;
using Shared.DataTransferObjects.UserRole;

namespace QuickStart
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // OrdersByLineDto: Ánh xạ từ một đối tượng tổng hợp (OrderLineDetail + Line)
            CreateMap<(OrderLineDetail OrderLineDetail, Line Line), OrdersByLineDto>()
                .ForMember(dest => dest.LineName, opt => opt.MapFrom(src => src.Line.LineName ?? "Unknown"))
                .ForMember(dest => dest.TotalOrders, opt => opt.Ignore()); // TotalOrders sẽ được tính trong service

            // OrderStatusTrendDto: Không cần ánh xạ trực tiếp vì là tổng hợp từ Order

            // TopProductDto: Ánh xạ từ một đối tượng tổng hợp (OrderDetail + ProductInformation)
            CreateMap<(OrderDetail OrderDetail, ProductInformation ProductInfo), TopProductDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductInfo.ProductName))
                .ForMember(dest => dest.TotalUnits, opt => opt.Ignore()); // TotalUnits sẽ được tính trong service

            // IncompleteOrderDto: Ánh xạ từ Order với dữ liệu tổng hợp
            CreateMap<Order, IncompleteOrderDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.ExportDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderCode))
                .ForMember(dest => dest.VehicleNumber, opt => opt.MapFrom(src => src.VehicleNumber ?? "N/A"))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.OrderDetails.First().ProductInformation.ProductName))
                .ForMember(dest => dest.RequestedUnits, opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.RequestedUnits)))
                .ForMember(dest => dest.ActualUnits, opt => opt.Ignore()) // ActualUnits cần SensorRecords, xử lý trong service
                .ForMember(dest => dest.CompletionPercentage, opt => opt.Ignore()); // Tính trong service

            // ProcessingOrderDto: Ánh xạ từ Order với dữ liệu tổng hợp
            CreateMap<(Order Order, OrderLineDetail OrderLineDetail, Line Line), ProcessingOrderDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Order.ExportDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderCode))
                .ForMember(dest => dest.VehicleNumber, opt => opt.MapFrom(src => src.Order.VehicleNumber ?? "N/A"))
                .ForMember(dest => dest.LineName, opt => opt.MapFrom(src => src.Line != null ? src.Line.LineName : "Unknown"))
                .ForMember(dest => dest.TotalUnits, opt => opt.Ignore()) // TotalUnits cần SensorRecords, xử lý trong service
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Processing"));

            // RecentCompletedOrderDto: Ánh xạ từ Order với dữ liệu tổng hợp
            CreateMap<(Order Order, Distributor Distributor, ProductInformation ProductInfo), RecentCompletedOrderDto>()
                .ForMember(dest => dest.CompletedDate, opt => opt.MapFrom(src => src.Order.ExportDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderCode))
                .ForMember(dest => dest.DistributorName, opt => opt.MapFrom(src => src.Distributor != null ? src.Distributor.DistributorName : "N/A"))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductInfo.ProductName))
                .ForMember(dest => dest.TotalUnits, opt => opt.Ignore()); // TotalUnits
            // Ánh xạ cho Area
            CreateMap<Area, AreaDto>();
            CreateMap<AreaForCreationDto, Area>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Distributors, opt => opt.Ignore());
            CreateMap<AreaForUpdateDto, Area>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Distributors, opt => opt.Ignore());

            // Ánh xạ cho Distributor
            CreateMap<Distributor, DistributorDto>()
    .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId))
    .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area));
            CreateMap<DistributorForCreationDto, Distributor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());
            CreateMap<DistributorForUpdateDto, Distributor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Area, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            // Ánh xạ cho InboundRecord
            CreateMap<InboundRecord, InboundRecordDto>()
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<InboundRecordForCreationDto, InboundRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());
            CreateMap<InboundRecordForUpdateDto, InboundRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());
            // Ánh xạ cho OutboundRecord
            CreateMap<OutboundRecord, OutboundRecordDto>()
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<OutboundRecordForCreationDto, OutboundRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());
            CreateMap<OutboundRecordForUpdateDto, OutboundRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());

            // Ánh xạ cho Line
            CreateMap<Line, LineDto>();
            CreateMap<LineForCreationDto, Line>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLineDetails, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());
            CreateMap<LineForUpdateDto, Line>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLineDetails, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());

            // Ánh xạ cho Order
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Distributor, opt => opt.MapFrom(src => src.Distributor));
            CreateMap<OrderForCreationDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Distributor, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLineDetails, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());
            CreateMap<OrderForUpdateDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Distributor, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.OrderLineDetails, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());

            // Ánh xạ cho OrderDetail
            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<OrderDetailForCreationDto, OrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());
            CreateMap<OrderDetailForUpdateDto, OrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.SensorRecords, opt => opt.Ignore());

            // Ánh xạ cho OrderLineDetail
            CreateMap<OrderLineDetail, OrderLineDetailDto>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.Line, opt => opt.MapFrom(src => src.Line));
            CreateMap<OrderLineDetailForCreationDto, OrderLineDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Line, opt => opt.Ignore());
            CreateMap<OrderLineDetailForUpdateDto, OrderLineDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Line, opt => opt.Ignore());

            // Ánh xạ cho Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.OrderDetail, opt => opt.MapFrom(src => src.OrderDetail))
                .ForMember(dest => dest.Distributor, opt => opt.MapFrom(src => src.Distributor))
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation)); // Thêm ánh xạ cho ProductInformation

            CreateMap<ProductForCreationDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetail, opt => opt.Ignore())
                .ForMember(dest => dest.Distributor, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore()); // Thêm Ignore cho ProductInformation

            CreateMap<ProductForUpdateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetail, opt => opt.Ignore())
                .ForMember(dest => dest.Distributor, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore()); // Thêm Ignore cho ProductInformation

            // Ánh xạ cho ProductInformation
            CreateMap<ProductInformation, ProductInformationDto>();
            CreateMap<ProductInformationForCreationDto, ProductInformation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore())
                .ForMember(dest => dest.InboundRecords, opt => opt.Ignore());
            CreateMap<ProductInformationForUpdateDto, ProductInformation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore())
                .ForMember(dest => dest.InboundRecords, opt => opt.Ignore());

            // Ánh xạ cho SensorRecord
            CreateMap<SensorRecord, SensorRecordDto>()
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.OrderDetail, opt => opt.MapFrom(src => src.OrderDetail))
                .ForMember(dest => dest.Line, opt => opt.MapFrom(src => src.Line));
            CreateMap<SensorRecordForCreationDto, SensorRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetail, opt => opt.Ignore())
                .ForMember(dest => dest.Line, opt => opt.Ignore());
            CreateMap<SensorRecordForUpdateDto, SensorRecord>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetail, opt => opt.Ignore())
                .ForMember(dest => dest.Line, opt => opt.Ignore());

            // Ánh xạ cho Stock
            CreateMap<InboundRecord, InboundRecordDto>()
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<InboundRecordForCreationDto, InboundRecord>();
            CreateMap<OutboundRecord, OutboundRecordDto>()
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<OutboundRecordForCreationDto, OutboundRecord>();
            CreateMap<ProductInformation, ProductInformationDto>();
            CreateMap<Stock, StockDto>();
            CreateMap<StockForCreationDto, Stock>();
            CreateMap<StockForUpdateDto, Stock>();
            CreateMap<Stock, StockDto>()
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));
            CreateMap<StockForCreationDto, Stock>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());
            CreateMap<StockForUpdateDto, Stock>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());

            // Ánh xạ cho phân quyền (giữ nguyên từ file gốc của bạn)
            CreateMap<RolePermission, RolePermissionDto>();
            CreateMap<RolePermissionForCreationDto, RolePermission>();
            CreateMap<RolePermissionForUpdateDto, RolePermission>();
            CreateMap<Permission, PermissionDto>();
            CreateMap<PermissionForCreationDto, Permission>();
            CreateMap<PermissionForUpdateDto, Permission>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryForCreationDto, Category>();
            CreateMap<CategoryForUpdateDto, Category>();
            CreateMap<User, UserDto>();
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<UserRoleForCreationDto, UserRole>();
            CreateMap<UserRoleForUpdateDto, UserRole>();
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<RolePermission, RoleMapPermissionDto>()
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission != null ? src.Permission.Name : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
        }
    }
}