using ECommercePlateform.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommercePlateform.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Coupen> Coupens { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVarient> ProductVarients { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default admin user
            SeedDefaultAdmin(modelBuilder);

            // Configure entity relationships

            // User entity configurations
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

                entity.HasMany(u => u.Carts)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.ShippingAddresses)
                    .WithOne(s => s.User)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // Address entity configurations
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

            // Cart entity configurations
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalItems).IsRequired();
                entity.Property(e => e.TotalAmount).IsRequired().HasPrecision(18, 2);

                entity.HasMany(c => c.CartItems)
                    .WithOne(ci => ci.Cart)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            // CartItem entity configurations
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);

                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ci => ci.ProductVarient)
                    .WithMany(pv => pv.CartItems)
                    .HasForeignKey(ci => ci.ProductVarientId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // City entity configurations
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.State)
                    .WithMany(s => s.Cities)
                    .HasForeignKey(c => c.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // Country entity configurations
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);

            });

            // Coupen entity configurations
            modelBuilder.Entity<Coupen>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DiscountValue).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.MinimumValue).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.MaximumValue).IsRequired().HasPrecision(18, 2);

                entity.HasOne(c => c.Product)
                    .WithMany(p => p.Coupens)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.ProductVarient)
                    .WithMany(pv => pv.Coupens)
                    .HasForeignKey(c => c.ProductVarientId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // Order entity configurations
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SubTotal).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.DiscountAmount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.TaxAmount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.ShippingAmount).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.TotalAmount).IsRequired().HasPrecision(18, 2);

                entity.HasOne(o => o.ShippingAddress)
                    .WithMany(sa => sa.Orders)
                    .HasForeignKey(o => o.ShippingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Coupen)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CoupenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            // OrderItem entity configurations
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).IsRequired().HasPrecision(18, 2);

                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.ProductVarient)
                    .WithMany(pv => pv.OrderItems)
                    .HasForeignKey(oi => oi.ProductVarientId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // Product entity configurations
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.BasePrice).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.Property(e => e.Tags).HasMaxLength(100);

                entity.HasMany(p => p.ProductVarients)
                    .WithOne(pv => pv.Product)
                    .HasForeignKey(pv => pv.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Reviews)
                    .WithOne(r => r.Product)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // ProductVarient entity configurations
            modelBuilder.Entity<ProductVarient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.Size).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.Property(e => e.Tags).HasMaxLength(100);
                entity.Property(e => e.StockQuantity).IsRequired();

            });

            // Review entity configurations
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.ReviewText).IsRequired().HasMaxLength(255);

            });

            // Setting entity configurations
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TaxRate).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.DeliveryFee).IsRequired().HasPrecision(18, 2);
                entity.Property(e => e.FreeDeliveryAbove).IsRequired().HasPrecision(18, 2);

            });

            // ShippingAddress entity configurations
            modelBuilder.Entity<ShippingAddress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Line1).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Line2).HasMaxLength(100);
                entity.Property(e => e.Line3).HasMaxLength(100);
                entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(18);

                entity.HasOne(sa => sa.City)
                    .WithMany(c => c.ShippingAddresses)
                    .HasForeignKey(sa => sa.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sa => sa.State)
                    .WithMany(s => s.ShippingAddresses)
                    .HasForeignKey(sa => sa.StateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sa => sa.Country)
                    .WithMany(c => c.ShippingAddresses)
                    .HasForeignKey(sa => sa.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            // State entity configurations
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

            // Handle circular references and self-references by ignoring some navigation properties
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var selfReferences = entityType.GetNavigations()
                    .Where(n => n.TargetEntityType.Name == entityType.Name);

                foreach (var selfReference in selfReferences)
                {
                    entityType.FindNavigation(selfReference.Name)?.SetField(null);
                    entityType.FindNavigation(selfReference.Name)?.SetPropertyAccessMode(PropertyAccessMode.Field);
                }
            }

            // Configure indexes for better query performance
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();
            modelBuilder.Entity<ProductVarient>().HasIndex(pv => pv.SKU).IsUnique();
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderNumber).IsUnique();
            modelBuilder.Entity<Coupen>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => new { c.Name, c.Code }).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.Code, s.Name, s.CountryId }).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.Code, s.CountryId }).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.Name, s.CountryId }).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => new { c.Name, c.StateId }).IsUnique();
        }

        private void SeedDefaultAdmin(ModelBuilder modelBuilder)
        {
            // Create a default admin user
            var adminId = Guid.Parse("E65A3A8A-2407-4965-9B71-B9A1D8E2C34F"); // Fixed GUID for admin

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@admin.com",
                    Password = "Admin@123", // In production, use a hashed password
                    ConfirmPassword = "Admin@123",
                    PhoneNumber = "1234567890",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Gender = Models.Enum.Gender.Male,
                    Bio = "System Administrator",
                    Role = Models.Enum.UserRole.Admin,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    IsActive = true,
                    IsDeleted = false
                }
            );
        }
    }
}

//// Ignore circular references
//entity.Ignore(e => e.Addresses);
//entity.Ignore(e => e.Carts);
//entity.Ignore(e => e.CartItems);
//entity.Ignore(e => e.Cities);
//entity.Ignore(e => e.Countries);
//entity.Ignore(e => e.Coupens);
//entity.Ignore(e => e.Orders);
//entity.Ignore(e => e.OrderItems);
//entity.Ignore(e => e.Products);
//entity.Ignore(e => e.ProductVarients);
//entity.Ignore(e => e.Reviews);
//entity.Ignore(e => e.Settings);
//entity.Ignore(e => e.ShippingAddresses);
//entity.Ignore(e => e.States);
//entity.Ignore(e => e.Users);