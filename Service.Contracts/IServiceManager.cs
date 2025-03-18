namespace Service.Contracts
{
    public interface IServiceManager
    {
        IAreaService AreaService { get; }
        ILineService LineService { get; }
        IDistributorService DistributorService { get; }
        IProductInformationService ProductInformationService { get; }
        IOrderService OrderService { get; }
        IOrderDetailService OrderDetailService { get; }
        IOrderLineDetailService OrderLineDetailService { get; }
        ISensorRecordService SensorRecordService { get; }
        IProductService ProductService { get; }
        IStockService StockService { get; }
        IInboundRecordService InboundRecordService { get; }
        IReportService ReportService { get; }
        // Các property cũ
        ICategoryService CategoryService { get; }
        IPermissionService PermissionService { get; }
        IRolePermissionService RolePermissionService { get; }
        IAuthenticationService AuthenticationService { get; }
        IAuthorizationServiceLocal AuthorizationService { get; }
        IUserService UserService { get; }
        IRoleService RoleService { get; }
        IAuditService AuditService { get; }
        IWcfService WcfService { get; }
        
    }
}