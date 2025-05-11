using ECommercePlateform.Server.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommercePlateform.Server.Infrastructure.Persistence.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.Property(pv => pv.Price)
                .HasPrecision(18, 2);
        }
    }
} 