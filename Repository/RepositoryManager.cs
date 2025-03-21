using Contracts;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IAreaRepository> _areaRepository;
        private readonly Lazy<ILineRepository> _lineRepository;
        private readonly Lazy<IDistributorRepository> _distributorRepository;
        private readonly Lazy<IProductInformationRepository> _productInformationRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IOrderDetailRepository> _orderDetailRepository;
        private readonly Lazy<IOrderLineDetailRepository> _orderLineDetailRepository;
        private readonly Lazy<ISensorRecordRepository> _sensorRecordRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<IStockRepository> _stockRepository;
        private readonly Lazy<IInboundRecordRepository> _inboundRecordRepository;
        private readonly Lazy<IOutboundRecordRepository> _outboundRecordRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IPermissionRepository> _permissionRepository;
        private readonly Lazy<IRolePermissionRepository> _rolePermissionRepository;
        private readonly Lazy<IAuditRepository> _auditRepository;

        public RepositoryManager(RepositoryContext repositoryContext, RoleManager<UserRole> roleManager)
        {
            _repositoryContext = repositoryContext;

            _areaRepository = new Lazy<IAreaRepository>(() => new AreaRepository(repositoryContext));
            _lineRepository = new Lazy<ILineRepository>(() => new LineRepository(repositoryContext));
            _distributorRepository = new Lazy<IDistributorRepository>(() => new DistributorRepository(repositoryContext));
            _productInformationRepository = new Lazy<IProductInformationRepository>(() => new ProductInformationRepository(repositoryContext));
            _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(repositoryContext));
            _orderDetailRepository = new Lazy<IOrderDetailRepository>(() => new OrderDetailRepository(repositoryContext));
            _orderLineDetailRepository = new Lazy<IOrderLineDetailRepository>(() => new OrderLineDetailRepository(repositoryContext));
            _sensorRecordRepository = new Lazy<ISensorRecordRepository>(() => new SensorRecordRepository(repositoryContext));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(repositoryContext));
            _stockRepository = new Lazy<IStockRepository>(() => new StockRepository(repositoryContext));
            _inboundRecordRepository = new Lazy<IInboundRecordRepository>(() => new InboundRecordRepository(repositoryContext));
            _outboundRecordRepository = new Lazy<IOutboundRecordRepository>(() => new OutboundRecordRepository(repositoryContext));

            // Khởi tạo các repository cũ
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(repositoryContext));
            _permissionRepository = new Lazy<IPermissionRepository>(() => new PermissionRepository(repositoryContext));
            _rolePermissionRepository = new Lazy<IRolePermissionRepository>(() => new RolePermissionRepository(repositoryContext));
            _auditRepository = new Lazy<IAuditRepository>(() => new AuditRepository(repositoryContext));
        }

        public IAreaRepository Area => _areaRepository.Value;
        public ILineRepository Line => _lineRepository.Value;
        public IDistributorRepository Distributor => _distributorRepository.Value;
        public IProductInformationRepository ProductInformation => _productInformationRepository.Value;
        public IOrderRepository Order => _orderRepository.Value;
        public IOrderDetailRepository OrderDetail => _orderDetailRepository.Value;
        public IOrderLineDetailRepository OrderLineDetail => _orderLineDetailRepository.Value;
        public ISensorRecordRepository SensorRecord => _sensorRecordRepository.Value;
        public IProductRepository Product => _productRepository.Value;
        public IStockRepository Stock => _stockRepository.Value;
        public IInboundRecordRepository InboundRecord => _inboundRecordRepository.Value;
        public IOutboundRecordRepository OutboundRecord => _outboundRecordRepository.Value;

        public ICategoryRepository Category => _categoryRepository.Value;
        public IPermissionRepository Permission => _permissionRepository.Value;
        public IRolePermissionRepository RolePermission => _rolePermissionRepository.Value;
        public IAuditRepository Audit => _auditRepository.Value;

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}