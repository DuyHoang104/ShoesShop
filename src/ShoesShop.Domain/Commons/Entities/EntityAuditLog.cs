using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Commons.Enums;

namespace ShoesShop.Domain.Commons.Entities;

public abstract class EntityAuditLog : AuditLog<int, LastAction>, IEntityAuditLog
{
}

public abstract class EntityAuditLog<TKey> : BaseEntity<TKey>, IEntityAuditLog<TKey>
    where TKey : struct
{
    public int CreateBy { get; set; }

    public DateTime CreateTimeStamp { get; set; }

    public int LastActionBy { get; set; }

    public LastAction LastAction { get; set; }

    public DateTime LastActionTimeStamp { get; set; }
}