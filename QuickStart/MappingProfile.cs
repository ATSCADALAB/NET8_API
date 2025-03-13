using AutoMapper;
using Entities.Identity;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.Account;
using Shared.DataTransferObjects.AuditLog;
using Shared.DataTransferObjects.Authentication;
using Shared.DataTransferObjects.Category;
using Shared.DataTransferObjects.Customer;
using Shared.DataTransferObjects.Distributor;
using Shared.DataTransferObjects.Order;
using Shared.DataTransferObjects.OrderLineDetail;
using Shared.DataTransferObjects.Permission;
using Shared.DataTransferObjects.Product;
using Shared.DataTransferObjects.ProductInformation;
using Shared.DataTransferObjects.RolePermission;
using Shared.DataTransferObjects.User;
using Shared.DataTransferObjects.UserRole;

namespace QuickStart
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Ánh xạ cho Order
            CreateMap<OrderLineDetail, OrderLineDetailDto>();
            CreateMap<OrderLineDetailForCreationDto, OrderLineDetail>();
            CreateMap<OrderLineDetailForUpdateDto, OrderLineDetail>();

            //Ánh xạ cho Order
            CreateMap<Order, OrderDto>()
               .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductInformation!.ProductCode))
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductInformation!.ProductName)) // Lấy thêm tên sản phẩm
               .ForMember(dest => dest.DistributorName, opt => opt.MapFrom(src => src.Distributor!.DistributorName))
               .ForMember(dest => dest.DistributorArea, opt => opt.MapFrom(src => src.Distributor!.Area)); // Lấy thêm khu vực của nhà phân phối
            CreateMap<OrderForCreationDto, Order>();
            CreateMap<OrderForUpdateDto, Order>();
            // Ánh xạ cho Product
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Distributor, opt => opt.MapFrom(src => src.Distributor))
                .ForMember(dest => dest.ProductInformation, opt => opt.MapFrom(src => src.ProductInformation));

            CreateMap<ProductForCreationDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id được tự động sinh
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Sử dụng giá trị mặc định
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Sử dụng giá trị mặc định
                .ForMember(dest => dest.Distributor, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore()); // Navigation property

            CreateMap<ProductForUpdateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Distributor, opt => opt.Ignore())
                .ForMember(dest => dest.ProductInformation, opt => opt.Ignore());

            // Ánh xạ cho Distributor
            CreateMap<Distributor, DistributorDto>();
            CreateMap<DistributorForCreationDto, Distributor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());
            CreateMap<DistributorForUpdateDto, Distributor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            // Ánh xạ cho ProductInformations
            CreateMap<ProductInformation, ProductInformationDto>();
            CreateMap<ProductInformationForCreationDto, ProductInformation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());
            CreateMap<ProductInformationForUpdateDto, ProductInformation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());
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
