using ECommercePlatform.Application.Interfaces.IUserAuth;
using ECommercePlatform.Domain.Entities;
using ECommercePlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;

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

        private static void SeedDefaultAdmin(ModelBuilder modelBuilder)
        {
            // Create a default admin user  
            var adminId = Guid.Parse("E65A3A8A-2407-4965-9B71-B9A1D8E2C34F"); // Fixed GUID for admin  

            // Use a specific fixed date instead of DateTime.Now  
            var fixedDate = new DateTime(2025, 5, 2, 3, 18, 0); // Year, Month, Day, Hour, Minute, Second  

            modelBuilder.Entity<User>().HasData(
                User.AdminCreate(
                    id: adminId, 
                    firstName: "Admin",
                    lastName: "User",
                    gender: Gender.Male,
                    dateOfBirth: new DateOnly(1990, 1, 1),
                    phoneNumber: "1234567890",
                    email: "admin@admin.com",
                    password: "Admin@123",
                    bio: "System Administrator",
                    userRole: UserRole.Admin,
                    isActive: true,
                    isDeleted: false,
                    createdOn: fixedDate,
                    createdBy: "System",
                    modifiedOn: fixedDate,
                    modifiedBy: "System"
                )
            );
        }
    }
}