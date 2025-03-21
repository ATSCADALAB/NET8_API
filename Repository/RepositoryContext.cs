using Entities.Identity;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repository.Configuration;
using System.Security.Principal;
using System.Text;

namespace Repository
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình các phần phân quyền người dùng
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Category)
                .WithMany()
                .HasForeignKey(rp => rp.CategoryId);

            // Cấu hình bảng Identity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductInformation)
                .WithMany(pi => pi.OrderDetails)
                .HasForeignKey(od => od.ProductInformationId);

            // OrderLineDetail
            modelBuilder.Entity<OrderLineDetail>()
                .HasOne(old => old.Order)
                .WithMany(o => o.OrderLineDetails)
                .HasForeignKey(old => old.OrderId);
            modelBuilder.Entity<OrderLineDetail>()
                .HasOne(old => old.Line)
                .WithMany(l => l.OrderLineDetails)
                .HasForeignKey(old => old.LineId);

            // Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.OrderDetail)
                .WithMany(od => od.Products)
                .HasForeignKey(p => p.OrderDetailId);
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Distributor)
                .WithMany(d => d.Products)
                .HasForeignKey(p => p.DistributorId);

            // SensorRecord
            modelBuilder.Entity<SensorRecord>()
                .HasOne(sr => sr.Order)
                .WithMany(o => o.SensorRecords)
                .HasForeignKey(sr => sr.OrderId);
            modelBuilder.Entity<SensorRecord>()
                .HasOne(sr => sr.OrderDetail)
                .WithMany(od => od.SensorRecords)
                .HasForeignKey(sr => sr.OrderDetailId);
            modelBuilder.Entity<SensorRecord>()
                .HasOne(sr => sr.Line)
                .WithMany(l => l.SensorRecords)
                .HasForeignKey(sr => sr.LineId);

            // Stock
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.ProductInformation)
                .WithOne(pi => pi.Stock)
                .HasForeignKey<Stock>(s => s.ProductInformationId);

            // Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Distributor)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DistributorId);

            // Distributor
            modelBuilder.Entity<Distributor>()
            .HasOne(d => d.Area)
            .WithMany(a => a.Distributors)
            .HasForeignKey(d => d.AreaId)
            .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa cascade nếu cần

            //Áp dụng configuration(nếu có)
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        // DbSet cho các bảng mới
        public DbSet<Area> Areas { get; set; } = default!;
        public DbSet<Line> Lines { get; set; } = default!;
        public DbSet<Distributor> Distributors { get; set; } = default!;
        public DbSet<ProductInformation> ProductInformations { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
        public DbSet<OrderLineDetail> OrderLineDetails { get; set; } = default!;
        public DbSet<SensorRecord> SensorRecords { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Stock> Stock { get; set; } = default!;
        public DbSet<InboundRecord> InboundRecords { get; set; } = default!;
        public DbSet<OutboundRecord> OutboundRecords { get; set; } = default!;

        // DbSet cho các bảng cũ
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Permission> Permissions { get; set; } = default!;
        public DbSet<RolePermission> RolePermissions { get; set; } = default!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            foreach (var modifiedEntity in modifiedEntities)
            {
                var auditLog = new AuditLog
                {
                    EntityName = modifiedEntity.Entity.GetType().Name,
                    Action = modifiedEntity.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Changes = GetChanges(modifiedEntity)
                };

                AuditLogs.Add(auditLog);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static string GetChanges(EntityEntry entity)
        {
            var changes = new StringBuilder();

            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];

                if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
                }
            }

            return changes.ToString();
        }
    }
}