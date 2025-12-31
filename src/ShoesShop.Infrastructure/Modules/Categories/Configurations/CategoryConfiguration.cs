using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Categories.Entities;

namespace ShoesShop.Infrastructure.Modules.Categories.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.Property(c => c.Name).HasField("_name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasField("_description").HasMaxLength(500).IsRequired(false);
        builder.Property(c => c.Status).HasField("_status").HasConversion<string>().IsRequired();

        builder.Property(c => c.LastActionTimeStamp).IsRequired();
        builder.Property(c => c.LastAction).HasConversion<string>().IsRequired();
        builder.Property(c => c.CreateBy).IsRequired();
        builder.Property(c => c.CreateTimeStamp).IsRequired();
        builder.Property(c => c.LastActionBy).IsRequired();

        builder.HasMany(c => c.ProductCategories).WithOne(pc => pc.Category).HasForeignKey(pc => pc.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}