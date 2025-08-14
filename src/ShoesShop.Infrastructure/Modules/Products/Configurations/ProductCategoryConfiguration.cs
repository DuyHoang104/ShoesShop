using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Products.Entities;

namespace ShoesShop.Infrastructure.Modules.Products.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        builder.Property(pc => pc.ProductId).HasField("_productId").IsRequired();
        builder.Property(pc => pc.CategoryId).HasField("_categoryId").IsRequired();

        builder.HasOne(pc => pc.Product).WithMany(p => p.ProductCategories).HasForeignKey(pc => pc.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pc => pc.Category).WithMany(c => c.ProductCategories).HasForeignKey(pc => pc.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}