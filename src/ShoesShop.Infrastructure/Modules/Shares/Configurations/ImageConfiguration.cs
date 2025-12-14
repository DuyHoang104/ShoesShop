using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Shares.Entities;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images");

        builder.Property(i => i.Url).IsRequired().HasMaxLength(2048);

        builder.HasOne(i => i.Product).WithMany(p => p.Images).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
