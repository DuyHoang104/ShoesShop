using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Products.Entities;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations;

public class ImageUploadConfiguration : IEntityTypeConfiguration<ImageUpload>
{
    public void Configure(EntityTypeBuilder<ImageUpload> builder)
    {
        builder.ToTable("ImageUploads");

        builder.Property(i => i.Url).IsRequired().HasMaxLength(2048);

        builder.HasOne(i => i.Product).WithMany(p => p.Images).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}
