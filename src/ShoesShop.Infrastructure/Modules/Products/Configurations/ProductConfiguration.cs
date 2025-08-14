using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Products.Entities;

namespace ShoesShop.Infrastructure.Modules.Products.Configurations;
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.Property(p => p.Name).HasField("_name").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasField("_description").HasMaxLength(500).IsRequired();
        builder.Property(p => p.Price).HasField("_price").IsRequired();
        builder.Property(p => p.Quantity).HasField("_quantity").IsRequired();
        builder.Property(p => p.ImageUrl).HasField("_imageUrl").IsRequired(false);
        builder.Property(p => p.SaleOff).HasField("_saleOff").IsRequired(false);
        builder.Property(p => p.Status).HasField("_status").IsRequired();

        builder.HasMany(p => p.OrderDetails).WithOne(od => od.Product).HasForeignKey(od => od.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.CartItems).WithOne(c => c.Product).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.ProductCategories).WithOne(pc => pc.Product).HasForeignKey(pc => pc.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}