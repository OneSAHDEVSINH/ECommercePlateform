using ECommercePlateform.Server.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommercePlateform.Server.Infrastructure.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.Property(c => c.DiscountValue)
                .HasPrecision(18, 2);

            builder.Property(c => c.MaximumValue)
                .HasPrecision(18, 2);

            builder.Property(c => c.MinimumValue)
                .HasPrecision(18, 2);
        }
    }
} 