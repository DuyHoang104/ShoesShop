using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Messages.Entity;

namespace ShoesShop.Infrastructure.Modules.Messages;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderId).IsRequired();
        builder.Property(m => m.ReceiverId).IsRequired();
        builder.Property(m => m.Content).IsRequired().HasMaxLength(1000);
        builder.Property(m => m.IsRead).IsRequired().HasDefaultValue(false);
        builder.Property(m => m.SentAt).IsRequired().HasDefaultValueSql("GETDATE()");
        builder.Property(m => m.SenderName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.SenderAvatar).IsRequired().HasMaxLength(500);
        builder.Property(m => m.SenderRole).IsRequired().HasMaxLength(100);
        builder.Property(m => m.OrderId).IsRequired();

        builder.Property(m => m.OrderId).IsRequired();

        builder
            .HasOne(m => m.Order)
            .WithMany(o => o.Messages)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.Order)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}