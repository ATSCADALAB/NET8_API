namespace Contracts
{
    public interface IRepositoryManager
    {
        IAuditRepository Audit { get; }
        ICustomerRepository Customer { get; }
        ICategoryRepository Category { get; }
        IPermissionRepository Permission { get; }
        IRolePermissionRepository RolePermission { get; }
        IAccountRepository Account { get; }
        Task SaveAsync();
    }
}
