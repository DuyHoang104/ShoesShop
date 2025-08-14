using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Orders.Entities;

namespace ShoesShop.Infrastructure.Modules.Orders.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");
        builder.Property(o => o.Quantity).HasField("_quantity").IsRequired();
        builder.Property(o => o.ProductId).HasField("_productId").IsRequired();
        builder.Property(o => o.OrderId).HasField("_orderId").IsRequired();

        builder.HasOne(o => o.Product).WithMany(p => p.OrderDetails).HasForeignKey(o => o.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o => o.Order).WithMany(o => o.OrderDetails).HasForeignKey(o => o.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}