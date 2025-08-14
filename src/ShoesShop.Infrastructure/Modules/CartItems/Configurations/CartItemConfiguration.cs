using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Carts.Entities;

namespace ShoesShop.Infrastructure.Modules.CartItems.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        builder.Property(c => c.Quantity).HasField("_quantity").IsRequired();
        builder.Property(c => c.ProductId).HasField("_productId").IsRequired();
        builder.Property(c => c.UserId).HasField("_userId").IsRequired();

        builder.HasOne(c => c.Product).WithMany(p => p.CartItems).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(c => c.User).WithMany(u => u.CartItems).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}