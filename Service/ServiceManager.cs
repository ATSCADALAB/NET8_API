using AutoMapper;
using Contracts;
using EmailService;
using Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR; // Thêm để dùng IHubContext
using Microsoft.Extensions.Configuration;
using QuickStart.Hubs; // Thêm để dùng DataHub
using QuickStart.Service;
using Service.Contracts;
using Service.JwtFeatures;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICategoryService> _categoryRepository;
        private readonly Lazy<IPermissionService> _permissionService;
        private readonly Lazy<IRolePermissionService> _rolePermissionService;
        private readonly Lazy<ICustomerService> _customerService;
        private readonly Lazy<IAccountService> _accountService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IAuthorizationServiceLocal> _authorization1Service;
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IDistributorService> _distributorService; // Thêm DistributorService
        private readonly Lazy<IProductInformationService> _productInformationService; // Thêm ProductInformationService
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
            IHubContext<DataHub> hubContext) // Thêm IHubContext<DataHub>
        {
            _authorization1Service = new Lazy<IAuthorizationServiceLocal>(() => new AuthorizationService(userManager, repositoryManager, logger));
            _categoryRepository = new Lazy<ICategoryService>(() => new CategoryService(repositoryManager, logger, mapper));
            _productService = new Lazy<IProductService>(() => new ProductService(repositoryManager, logger, mapper));
            _distributorService = new Lazy<IDistributorService>(() => new DistributorService(repositoryManager, logger, mapper));
            _productInformationService = new Lazy<IProductInformationService>(() => new ProductInformationService(repositoryManager, logger, mapper));
            _permissionService = new Lazy<IPermissionService>(() => new PermissionService(repositoryManager, logger, mapper));
            _rolePermissionService = new Lazy<IRolePermissionService>(() => new RolePermissionService(repositoryManager, logger, mapper));
            _customerService = new Lazy<ICustomerService>(() => new CustomerService(repositoryManager, logger, mapper));
            _accountService = new Lazy<IAccountService>(() => new AccountService(repositoryManager, logger, mapper));
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                logger, mapper, userManager, configuration, jwtHandler, emailSender));
            _userService = new Lazy<IUserService>(() => new UserService(logger, mapper, userManager));
            _roleService = new Lazy<IRoleService>(() => new RoleService(logger, mapper, roleManager));
            _auditService = new Lazy<IAuditService>(() => new AuditService(repositoryManager, logger, mapper));
            _wcfService = new Lazy<IWcfService>(() => new WcfService(configuration, hubContext)); // Truyền hubContext vào WcfService
        }

        public IWcfService WcfService => _wcfService.Value;
        public ICustomerService CustomerService => _customerService.Value;
        public IProductService ProductService => _productService.Value;
        public IDistributorService DistributorService => _distributorService.Value; // Thêm property
        public IProductInformationService ProductInformationService => _productInformationService.Value; // Thêm property
        public ICategoryService CategoryService => _categoryRepository.Value;
        public IPermissionService PermissionService => _permissionService.Value;
        public IRolePermissionService RolePermissionService => _rolePermissionService.Value;
        public IAccountService AccountService => _accountService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAuthorizationServiceLocal AuthorizationService => _authorization1Service.Value;
        public IUserService UserService => _userService.Value;
        public IRoleService RoleService => _roleService.Value;
        public IAuditService AuditService => _auditService.Value;
    }
}