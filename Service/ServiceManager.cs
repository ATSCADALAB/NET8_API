using AutoMapper;
using Contracts;
using EmailService;
using Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR; // Dùng cho IHubContext
using Microsoft.Extensions.Configuration;
using QuickStart.Hubs; // Dùng cho DataHub
using QuickStart.Service;
using Service.Contracts;
using Service.JwtFeatures;
using System;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAreaService> _areaService;
        private readonly Lazy<IReportService> _reportService;
        private readonly Lazy<ILineService> _lineService;
        private readonly Lazy<IDistributorService> _distributorService;
        private readonly Lazy<IProductInformationService> _productInformationService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<IOrderDetailService> _orderDetailService;
        private readonly Lazy<IOrderLineDetailService> _orderLineDetailService;
        private readonly Lazy<ISensorRecordService> _sensorRecordService;
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IStockService> _stockService;
        private readonly Lazy<IInboundRecordService> _inboundRecordService;
        private readonly Lazy<IOutboundRecordService> _outboundRecordService;
        private readonly Lazy<IDashboardService> _dashboardService;

        // Các service cũ từ ví dụ của bạn
        private readonly Lazy<ICategoryService> _categoryService;
        private readonly Lazy<IPermissionService> _permissionService;
        private readonly Lazy<IRolePermissionService> _rolePermissionService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IAuthorizationServiceLocal> _authorizationService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IRoleService> _roleService;
        private readonly Lazy<IAuditService> _auditService;
        private readonly Lazy<IWcfService> _wcfService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            ILoggerManager logger,
            IMapper mapper,
            UserManager<User> userManager,
            IConfiguration configuration,
            RoleManager<UserRole> roleManager,
            JwtHandler jwtHandler,
            IEmailSender emailSender,
            IHubContext<DataHub> hubContext)
        {
            _dashboardService = new Lazy<IDashboardService>(() => new DashboardService(repositoryManager, logger, mapper));
            _areaService = new Lazy<IAreaService>(() => new AreaService(repositoryManager, logger, mapper));
            _lineService = new Lazy<ILineService>(() => new LineService(repositoryManager, logger, mapper));
            _distributorService = new Lazy<IDistributorService>(() => new DistributorService(repositoryManager, logger, mapper));
            _productInformationService = new Lazy<IProductInformationService>(() => new ProductInformationService(repositoryManager, logger, mapper));
            _orderService = new Lazy<IOrderService>(() => new OrderService(repositoryManager, logger, mapper));
            _orderDetailService = new Lazy<IOrderDetailService>(() => new OrderDetailService(repositoryManager, logger, mapper));
            _orderLineDetailService = new Lazy<IOrderLineDetailService>(() => new OrderLineDetailService(repositoryManager, logger, mapper));
            _sensorRecordService = new Lazy<ISensorRecordService>(() => new SensorRecordService(repositoryManager, logger, mapper));
            _productService = new Lazy<IProductService>(() => new ProductService(repositoryManager, logger, mapper));
            _stockService = new Lazy<IStockService>(() => new StockService(repositoryManager, logger, mapper));
            _inboundRecordService = new Lazy<IInboundRecordService>(() => new InboundRecordService(repositoryManager, logger, mapper));
            _outboundRecordService = new Lazy<IOutboundRecordService>(() => new OutboundRecordService(repositoryManager, logger, mapper));
            _reportService = new Lazy<IReportService>(() =>  new ReportService(repositoryManager, logger, mapper, configuration));        // Khởi tạo các service cũ
            _categoryService = new Lazy<ICategoryService>(() => new CategoryService(repositoryManager, logger, mapper));
            _permissionService = new Lazy<IPermissionService>(() => new PermissionService(repositoryManager, logger, mapper));
            _rolePermissionService = new Lazy<IRolePermissionService>(() => new RolePermissionService(repositoryManager, logger, mapper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                logger, mapper, userManager, configuration, jwtHandler, emailSender));
            _authorizationService = new Lazy<IAuthorizationServiceLocal>(() => new AuthorizationService(userManager, repositoryManager, logger));
            _userService = new Lazy<IUserService>(() => new UserService(logger, mapper, userManager));
            _roleService = new Lazy<IRoleService>(() => new RoleService(logger, mapper, roleManager));
            _auditService = new Lazy<IAuditService>(() => new AuditService(repositoryManager, logger, mapper));
            _wcfService = new Lazy<IWcfService>(() => new WcfService(configuration, hubContext));
        }

        public IDashboardService DashboardService => _dashboardService.Value;
        public IAreaService AreaService => _areaService.Value;
        public ILineService LineService => _lineService.Value;
        public IDistributorService DistributorService => _distributorService.Value;
        public IProductInformationService ProductInformationService => _productInformationService.Value;
        public IOrderService OrderService => _orderService.Value;
        public IOrderDetailService OrderDetailService => _orderDetailService.Value;
        public IOrderLineDetailService OrderLineDetailService => _orderLineDetailService.Value;
        public ISensorRecordService SensorRecordService => _sensorRecordService.Value;
        public IProductService ProductService => _productService.Value;
        public IStockService StockService => _stockService.Value;
        public IInboundRecordService InboundRecordService => _inboundRecordService.Value;
        public IOutboundRecordService OutboundRecordService => _outboundRecordService.Value;
        public IReportService ReportService => _reportService.Value;
        // Property cho các service cũ
        public ICategoryService CategoryService => _categoryService.Value;
        public IPermissionService PermissionService => _permissionService.Value;
        public IRolePermissionService RolePermissionService => _rolePermissionService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAuthorizationServiceLocal AuthorizationService => _authorizationService.Value;
        public IUserService UserService => _userService.Value;
        public IRoleService RoleService => _roleService.Value;
        public IAuditService AuditService => _auditService.Value;
        public IWcfService WcfService => _wcfService.Value;
    }
}