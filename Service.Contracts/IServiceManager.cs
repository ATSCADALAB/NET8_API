namespace Service.Contracts
{
    public interface IServiceManager
    {
        IAuditService AuditService { get; }
        IAuthenticationService AuthenticationService { get; }
        IAuthorizationServiceLocal AuthorizationService { get; }
        ICustomerService CustomerService { get; }
        ICategoryService CategoryService { get; }
        IPermissionService PermissionService { get; }
        IRolePermissionService RolePermissionService { get; }
        IAccountService AccountService { get; }
        IUserService UserService { get; }
        IRoleService RoleService { get; }
        IWcfService WcfService { get; }
    }
}
