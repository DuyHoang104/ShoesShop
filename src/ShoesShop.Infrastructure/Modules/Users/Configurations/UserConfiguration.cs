using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Users.Entities;

namespace ShoesShop.Infrastructure.Modules.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(e => e.UserName).HasField("_userName").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Password).HasField("_password").HasMaxLength(100).IsRequired();
        builder.Property(e => e.DateOfBirth).HasField("_dateOfBirth").IsRequired();
        builder.Property(e => e.Email).HasField("_email").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Phone).HasField("_phone").HasMaxLength(20);
        builder.Property(e => e.Role).HasField("_role").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Gender).HasField("_gender").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Status).HasField("_status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.AvatarUrl).HasField("_avatarUrl").HasMaxLength(2048);
        
        builder.HasMany(e => e.Addresses).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Orders).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.CartItems).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}