namespace Contracts
{
    public interface IRepositoryManager
    {
        IAuditRepository Audit { get; }
        ICustomerRepository Customer { get; }
        IProductRepository Product { get; }
        IDistributorRepository Distributor { get; } // Thêm Distributor
        IProductInformationRepository ProductInformation { get; } // Thêm ProductInformation
        ICategoryRepository Category { get; }
        IOrderRepository Order { get; }
        IPermissionRepository Permission { get; }
        IRolePermissionRepository RolePermission { get; }
        IAccountRepository Account { get; }
        Task SaveAsync();
    }
}
