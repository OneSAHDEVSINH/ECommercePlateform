using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using static ECommercePlatform.Domain.Entities.Permission;

namespace ECommercePlatform.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : DbContext(options)
    {
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Module> Modules { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var now = DateTime.Now;
            var userId = _currentUserService.IsAuthenticated
                ? _currentUserService.UserId ?? _currentUserService.Email ?? "system"
                : "system";

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = now;
                    //entry.Entity.ModifiedBy = userId;
                    //entry.Entity.ModifiedOn = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Don't modify creation fields
                    //entry.Property(e => e.CreatedBy).IsModified = false;
                    //entry.Property(e => e.CreatedOn).IsModified = false;

                    // Update modification fields
                    entry.Entity.ModifiedBy = userId;
                    entry.Entity.ModifiedOn = now;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default admin user
            SeedDefaultAdmin(modelBuilder);

            // Apply entity configurations
            ApplyEntityConfigurations(modelBuilder);
        }

        private static void ApplyEntityConfigurations(ModelBuilder modelBuilder)
        {
            // Configure entity relationships
            ConfigureUserEntity(modelBuilder);
            ConfigureAddressEntity(modelBuilder);
            ConfigureCityEntity(modelBuilder);
            ConfigureCountryEntity(modelBuilder);
            ConfigureStateEntity(modelBuilder);
            ConfigureOrderEntity(modelBuilder);
            ConfigureProductEntity(modelBuilder);
            ConfigureCategoryEntity(modelBuilder);
            ConfigureUserRoleRelationships(modelBuilder);
            ConfigureCouponEntity(modelBuilder);
            ConfigureOrderItemEntity(modelBuilder);
            ConfigureProductVariantEntity(modelBuilder);
            // Add more configuration methods as needed
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(254);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);

                entity.HasMany(u => u.Addresses)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureAddressEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Line1).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Line2).HasMaxLength(100);
                entity.Property(e => e.Line3).HasMaxLength(100);
                entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(18);

                entity.HasOne(a => a.City)
                    .WithMany(c => c.Addresses)
                    .HasForeignKey(a => a.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.State)
                    .WithMany(s => s.Addresses)
                    .HasForeignKey(a => a.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Country)
                    .WithMany(c => c.Addresses)
                    .HasForeignKey(a => a.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCityEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.State)
                    .WithMany(s => s.Cities)
                    .HasForeignKey(c => c.StateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCountryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            });
        }

        private static void ConfigureStateEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);

                entity.HasOne(s => s.Country)
                    .WithMany(c => c.States)
                    .HasForeignKey(s => s.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureOrderEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubTotal).HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
                entity.Property(e => e.ShippingAmount).HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.HasOne(o => o.ShippingAddress)
                    .WithMany(sa => sa.Orders)
                    .HasForeignKey(o => o.ShippingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Coupon)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CouponId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureCategoryEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(c => c.ParentCategory)
                    .WithMany(c => c.Subcategories)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureUserRoleRelationships(ModelBuilder modelBuilder)
        {
            //// Explicitly ignore the problematic property
            //modelBuilder.Entity<User>()
            //    .Ignore(u => u.Role);

            // Configure the many-to-many relationship through UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.Id);

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureCouponEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Code).IsRequired();

                // Add precision/scale to decimal properties
                entity.Property(e => e.DiscountValue).HasPrecision(18, 2);
                entity.Property(e => e.MinimumValue).HasPrecision(18, 2);
                entity.Property(e => e.MaximumValue).HasPrecision(18, 2);
            });
        }

        private static void ConfigureOrderItemEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Add precision/scale to decimal properties
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.Discount).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            });
        }

        private static void ConfigureProductVariantEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Add precision/scale to decimal properties
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });
        }

        private static void SeedDefaultAdmin(ModelBuilder modelBuilder)
        {
            // Create fixed GUIDs for entities
            var adminUserId = Guid.Parse("E65A3A8A-2407-4965-9B71-B9A1D8E2C34F");
            var adminRoleId = Guid.Parse("D4DE1B4D-B43B-4A55-B47A-1E92E71C3143");
            var userRoleId = Guid.Parse("F8B7B597-14FF-4B33-A8B3-0EA4DE9F9DAE");
            var fixedDate = new DateTime(2025, 5, 2, 3, 18, 0);

            // Seed admin role
            modelBuilder.Entity<Role>().HasData(
                new
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    Description = "Administrator role with all permissions",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new
                {
                    Id = adminUserId,
                    FirstName = "Admin",
                    LastName = "User",
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    PhoneNumber = "1234567890",
                    Email = "admin@admin.com",
                    Password = "Admin@1234",
                    Bio = "System Administrator",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Seed the user-role relationship
            modelBuilder.Entity<UserRole>().HasData(
                new
                {
                    Id = userRoleId,
                    UserId = adminUserId,
                    RoleId = adminRoleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Module IDs
            var dashboardModuleId = Guid.Parse("8160be48-f4fd-4905-879b-e8038d64fde8");
            var usersModuleId = Guid.Parse("5d24ad3c-1189-43cc-a823-e882d84edb53");
            var rolesModuleId = Guid.Parse("666c62d8-94fd-4d1e-a98c-d783e97bdbac");
            var countriesModuleId = Guid.Parse("d5212365-524a-4afc-a44b-c1436c48f3a5");
            var statesModuleId = Guid.Parse("a7b3d254-9047-405f-aef3-7f9a6ed13c54");
            var citiesModuleId = Guid.Parse("27786d06-cdc7-4c27-a6a4-aac1622b110b");

            // Seed default modules
            modelBuilder.Entity<Module>().HasData(
                new
                {
                    Id = dashboardModuleId,
                    Name = "Dashboard",
                    Route = "dashboard",
                    Description = "Main admin dashboard",
                    DisplayOrder = 1,
                    Icon = "fas fa-tachometer-alt",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = usersModuleId,
                    Name = "Users",
                    Route = "users",
                    Description = "User management",
                    DisplayOrder = 2,
                    Icon = "fas fa-users",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = rolesModuleId,
                    Name = "Roles",
                    Route = "roles",
                    Description = "Role management",
                    DisplayOrder = 3,
                    Icon = "fas fa-user-shield",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = countriesModuleId,
                    Name = "Countries",
                    Route = "countries",
                    Description = "Country management",
                    DisplayOrder = 4,
                    Icon = "fas fa-globe",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = statesModuleId,
                    Name = "States",
                    Route = "states",
                    Description = "State management",
                    DisplayOrder = 5,
                    Icon = "fas fa-map",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = citiesModuleId,
                    Name = "Cities",
                    Route = "cities",
                    Description = "City management",
                    DisplayOrder = 6,
                    Icon = "fas fa-city",
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Permission IDs
            var dashboardViewId = Guid.Parse("565c7647-7611-4d34-ae76-5eba0d4e1822");
            var usersViewId = Guid.Parse("bf2f6ca5-dac3-4725-9407-c713a88ed19b");
            var rolesViewId = Guid.Parse("b684f1b4-0c54-466f-ba92-5e575061318b");
            var countriesViewId = Guid.Parse("d45768db-7234-4c97-aabd-0e8a74548138");
            var statesViewId = Guid.Parse("87104812-ebf5-45df-8f1c-41ef41a2d1de");
            var citiesViewId = Guid.Parse("c35b8160-2ef9-4936-8913-c35a5ac95a03");

            // Seed basic permissions with enum values
            modelBuilder.Entity<Permission>().HasData(
                new
                {
                    Id = dashboardViewId,
                    Name = "View Dashboard",
                    Description = "Permission to view the admin dashboard",
                    Type = PermissionType.View,
                    ModuleId = dashboardModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = usersViewId,
                    Name = "View Users",
                    Description = "Permission to view users",
                    Type = PermissionType.View,
                    ModuleId = usersModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = rolesViewId,
                    Name = "View Roles",
                    Description = "Permission to view roles",
                    Type = PermissionType.View,
                    ModuleId = rolesModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = countriesViewId,
                    Name = "View Countries",
                    Description = "Permission to view countries",
                    Type = PermissionType.View,
                    ModuleId = countriesModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = statesViewId,
                    Name = "View States",
                    Description = "Permission to view states",
                    Type = PermissionType.View,
                    ModuleId = statesModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = citiesViewId,
                    Name = "View Cities",
                    Description = "Permission to view cities",
                    Type = PermissionType.View,
                    ModuleId = citiesModuleId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Assign permissions to admin role
            modelBuilder.Entity<RolePermission>().HasData(
                new
                {
                    Id = Guid.Parse("54477EED-7960-4F78-9212-D6B3446A3553"),
                    RoleId = adminRoleId,
                    PermissionId = dashboardViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("5FD34CD1-368F-4FDA-A626-79C2E5C37B1A"),
                    RoleId = adminRoleId,
                    PermissionId = usersViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("C16E7C9B-ED37-401D-BF52-4C52BE030451"),
                    RoleId = adminRoleId,
                    PermissionId = rolesViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("264C6B33-9C91-4B01-A2BE-243D9F91110C"),
                    RoleId = adminRoleId,
                    PermissionId = countriesViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("7ECD8D51-8077-4F1C-B83D-9A2C6B9E20EA"),
                    RoleId = adminRoleId,
                    PermissionId = statesViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("75137078-D567-4F2F-9BE7-6F6E8BDCA429"),
                    RoleId = adminRoleId,
                    PermissionId = citiesViewId,
                    IsActive = true,
                    CreatedBy = "System",
                    CreatedOn = fixedDate,
                    ModifiedBy = "System",
                    ModifiedOn = fixedDate,
                    IsDeleted = false
                }
            );

            // Optionally: Seed all permissions and assign to admin role for full access
            // (If you want to guarantee admin always has all permissions by default)
            // You can add logic here to seed all permissions and role-permissions if needed.
        }
    }
}