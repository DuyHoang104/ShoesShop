using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Modules.Commons.Enums;

namespace ShoesShop.Domain.Modules.Commons.Entities
{
    public interface IEntityLastActionLog : IBaseEntity, IAuditLastActionLog<int, LastAction>
    {
    }

    public interface IEntityLastActionLog<TKey> : IBaseEntity<TKey>, IEntityLastActionLog 
        where TKey : struct
    {
    }
}