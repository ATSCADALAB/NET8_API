using AutoMapper;
using Entities.Identity;
using Entities.Models;
using Shared.DataTransferObjects.Account;
using Shared.DataTransferObjects.AuditLog;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Category;
using Shared.DataTransferObjects.Customer;
using Shared.DataTransferObjects.Permission;
using Shared.DataTransferObjects.RolePermission;
using Shared.DataTransferObjects.User;
using Shared.DataTransferObjects.UserRole;

namespace QuickStart
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Các chức năng Phân quyền 
            CreateMap<RolePermission, RolePermissionDto>();
            CreateMap<RolePermission, RolePermissionForCreationDto>();
            CreateMap<RolePermissionForCreationDto, RolePermission>();
            CreateMap<RolePermissionForUpdateDto, RolePermission>();
            CreateMap<PermissionForCreationDto, Permission>();
            CreateMap<Permission, PermissionDto>();
            CreateMap<PermissionForCreationDto, Permission>();
            CreateMap<PermissionForUpdateDto, Permission>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryForCreationDto, Category>();
            CreateMap<CategoryForUpdateDto, Category>();
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.accountCount, o => o.MapFrom(s => s.Accounts.Count));
            CreateMap<Account, AccountDto>();
            CreateMap<AccountForCreationDto, AccountDto>();
            CreateMap<CustomerForCreationDto, Customer>();
            CreateMap<AccountForCreationDto, Account>();
            CreateMap<AccountForUpdateDto, Account>().ReverseMap();
            CreateMap<CustomerForUpdateDto, Customer>();
            CreateMap<UserForRegistrationDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<UserRole, UserRoleDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserRoleForCreationDto, UserRole>();
            CreateMap<UserRoleForUpdateDto, UserRole>();
            CreateMap<AuditLog, AuditLogDto>();
            // Thêm ánh xạ mới từ RolePermission sang RoleMapPermissionDto
            CreateMap<RolePermission, RoleMapPermissionDto>()
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src => src.Permission.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
