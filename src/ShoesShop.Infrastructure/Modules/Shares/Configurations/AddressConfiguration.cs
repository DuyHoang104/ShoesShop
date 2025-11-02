using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Shares.Entities;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");
        builder.Property(e => e.AddressLine1).HasField("_addressLine1").HasMaxLength(100).IsRequired();
        builder.Property(e => e.City).HasField("_city").HasMaxLength(100);
        builder.Property(e => e.Country).HasField("_country").HasMaxLength(100);
        builder.Property(e => e.IsDefault).HasDefaultValue(false);

        builder.HasMany(e => e.Orders).WithOne(e => e.Address).HasForeignKey(e => e.AddressId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(e => e.User).WithMany(e => e.Addresses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}