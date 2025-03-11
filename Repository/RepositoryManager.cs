using Contracts;
using Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<ICustomerRepository> _customerRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<IDistributorRepository> _distributorRepository; // Thêm DistributorRepository
        private readonly Lazy<IProductInformationRepository> _productInformationRepository; // Thêm ProductInformationRepository
        private readonly Lazy<IPermissionRepository> _permissionRepository;
        private readonly Lazy<IRolePermissionRepository> _rolePermissionRepository;
        private readonly Lazy<IAccountRepository> _accountRepository;
        private readonly Lazy<IAuditRepository> _auditRepository;

        public RepositoryManager(RepositoryContext repositoryContext, RoleManager<UserRole> roleManager)
        {
            _repositoryContext = repositoryContext;
            _customerRepository = new Lazy<ICustomerRepository>(() => new CustomerRepository(repositoryContext));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(repositoryContext));
            _distributorRepository = new Lazy<IDistributorRepository>(() => new DistributorRepository(repositoryContext));
            _productInformationRepository = new Lazy<IProductInformationRepository>(() => new ProductInformationRepository(repositoryContext));
            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(repositoryContext));
            _permissionRepository = new Lazy<IPermissionRepository>(() => new PermissionRepository(repositoryContext));
            _rolePermissionRepository = new Lazy<IRolePermissionRepository>(() => new RolePermissionRepository(repositoryContext));
            _accountRepository = new Lazy<IAccountRepository>(() => new AccountRepository(repositoryContext));
            _auditRepository = new Lazy<IAuditRepository>(() => new AuditRepository(repositoryContext));


        }

        public ICustomerRepository Customer => _customerRepository.Value;
        public IProductRepository Product => _productRepository.Value;
        public IDistributorRepository Distributor => _distributorRepository.Value; // Thêm property
        public IProductInformationRepository ProductInformation => _productInformationRepository.Value; // Thêm property
        public ICategoryRepository Category => _categoryRepository.Value;
        public IPermissionRepository Permission => _permissionRepository.Value;
        public IRolePermissionRepository RolePermission => _rolePermissionRepository.Value;
        public IAccountRepository Account => _accountRepository.Value;
        public IAuditRepository Audit => _auditRepository.Value;

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}
