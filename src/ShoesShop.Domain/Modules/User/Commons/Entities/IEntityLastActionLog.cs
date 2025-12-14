using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Modules.User.Commons.Enums;

namespace ShoesShop.Domain.Modules.User.Commons.Entities
{
    public interface IEntityLastActionLog : IBaseEntity, IAuditLastActionLog<int, LastAction>
    {
    }

    public interface IEntityLastActionLog<TKey> : IBaseEntity<TKey>, IEntityLastActionLog 
        where TKey : struct
    {
    }
}