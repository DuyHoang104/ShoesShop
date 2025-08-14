using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Categories.Entities;

namespace ShoesShop.Infrastructure.Modules.Categories.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.Property(c => c.Name).HasField("_name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasField("_description").HasMaxLength(500).IsRequired(false);

        builder.HasMany(c => c.ProductCategories).WithOne(pc => pc.Category).HasForeignKey(pc => pc.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}