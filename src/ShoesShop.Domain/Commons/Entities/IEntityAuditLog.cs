using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Commons.Enums;

namespace ShoesShop.Domain.Commons.Entities;
public interface IEntityAuditLog : IAuditLog<int, LastAction>, IEntityCreatedLog, IEntityLastActionLog
{
}

public interface IEntityAuditLog<TKey> : IEntityAuditLog , IEntityCreatedLog<TKey>, IEntityLastActionLog<TKey>
    where TKey : struct
{
}