using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.User.Carts.Entities;

namespace ShoesShop.Infrastructure.Modules.Carts.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.Property(c => c.Quantity).HasField("_quantity").IsRequired();
        builder.Property(c => c.ProductId).HasField("_productId").IsRequired();
        builder.Property(c => c.UserId).HasField("_userId").IsRequired();

        builder.HasOne(c => c.Product).WithMany(p => p.Carts).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(c => c.User).WithMany(u => u.Carts).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}