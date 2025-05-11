using ECommercePlateform.Server.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommercePlateform.Server.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.Discount)
                .HasPrecision(18, 2);

            builder.Property(oi => oi.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
} 