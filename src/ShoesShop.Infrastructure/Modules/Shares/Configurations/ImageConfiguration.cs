using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Shares.Image.Entities;
using ShoesShop.Domain.Modules.Shares.Image.Enums;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(2048);

        builder.Property(i => i.OwnerType)
                .HasConversion<string>()
                .IsRequired();

        builder.Property(x => x.OwnerId)
                .IsRequired();
        
        builder.Property(x => x.PublicId)
                .IsRequired()
                .HasMaxLength(512);

        builder.HasDiscriminator(i => i.OwnerType)
                .HasValue<ImageUser>(OwnerType.User)
                .HasValue<ImageProduct>(OwnerType.Product)
                .HasValue<ImageReview>(OwnerType.Review);
    }
}