using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Orders.Entities;

namespace ShoesShop.Infrastructure.Modules.Orders.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(e => e.OrderDate).HasField("_orderDate").IsRequired().HasDefaultValueSql("GETDATE()");
        builder.Property(e => e.Status).HasConversion<string>().HasField("_status").IsRequired();
        builder.Property(e => e.PaymentMethod).HasConversion<string>().HasField("_paymentMethod").IsRequired();
        builder.Property(e => e.PaymentStatus).HasConversion<string>().HasField("_paymentStatus").IsRequired();
        builder.Property(e => e.Note).HasField("_note").HasMaxLength(255);
        builder.Property(e => e.ReceiverName).HasField("_receiverName").HasMaxLength(100);
        builder.Property(e => e.ReceiverPhone).HasField("_receiverPhone").HasMaxLength(15);
        builder.Property(e => e.ReceiverAddress).HasField("_receiverAddress").HasMaxLength(255);
        builder.Property(e => e.ShippingFee).HasField("_shippingFee");
        builder.Property(e => e.Discount).HasField("_discount");

        builder.HasMany(e => e.OrderDetails).WithOne(e => e.Order).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.User).WithMany(e => e.Orders).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Address).WithMany(e => e.Orders).HasForeignKey(e => e.AddressId).IsRequired().OnDelete(DeleteBehavior.NoAction);
    }
}