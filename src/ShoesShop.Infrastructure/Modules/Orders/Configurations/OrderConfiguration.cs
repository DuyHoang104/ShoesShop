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
        builder.Property(e => e.Status).HasField("_status");
        builder.Property(e => e.TotalAmount).HasField("_totalAmount").IsRequired();
        builder.Property(e => e.PaymentMethod).HasField("_paymentMethod");
        builder.Property(e => e.PaymentStatus).HasField("_paymentStatus");
        builder.Property(e => e.AddressId).HasField("_addressId").IsRequired();
        builder.Property(e => e.Note).HasField("_note").HasMaxLength(255);

        builder.HasMany(e => e.OrderDetails).WithOne(e => e.Order).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.User).WithMany(e => e.Orders).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Address).WithMany(e => e.Orders).HasForeignKey(e => e.AddressId).IsRequired().OnDelete(DeleteBehavior.NoAction);
    }
}