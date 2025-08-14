using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoesShop.Domain.Modules.Shares.Entities;

namespace ShoesShop.Infrastructure.Modules.Shares.Configurations;

public class ObjectChangeLogConfiguration : IEntityTypeConfiguration<ObjectChangeLog>
{
    public void Configure(EntityTypeBuilder<ObjectChangeLog> builder)
    {
        builder.ToTable("ObjectChangeLogs");
        builder.Property(e => e.EntityName).HasField("_entityName").IsRequired();
        builder.Property(e => e.PropertyName).HasField("_propertyName").IsRequired();
        builder.Property(e => e.KeyValue).HasField("_keyValue").IsRequired();
        builder.Property(e => e.OldValue).HasField("_oldValue");
        builder.Property(e => e.NewValue).HasField("_newValue");
        builder.Property(e => e.ChangeDate).HasField("_changeDate").IsRequired();
        builder.Property(e => e.UserId).HasField("_userId").IsRequired();
        
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
    }
}