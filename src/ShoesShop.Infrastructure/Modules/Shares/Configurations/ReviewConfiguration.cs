using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Shares.Review.Entity;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.ParentId)
                .IsRequired(false);

            builder.HasOne(r => r.Parent)
                .WithMany(r => r.Children)
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Metadata)
                .IsRequired(false)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    v => JsonSerializer.Deserialize<object>(v, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                )
                .HasColumnType("nvarchar(max)");

            builder.HasMany(e => e.Images)
                .WithOne(x => x.Review)
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Images)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(x => x.CreateBy).IsRequired();
            builder.Property(x => x.CreateTimeStamp).IsRequired();
            builder.Property(x => x.LastActionBy).IsRequired();
            builder.Property(x => x.LastAction).IsRequired();
            builder.Property(x => x.LastActionTimeStamp).IsRequired();
        }
    }
}