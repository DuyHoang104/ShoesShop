using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Modules.User.Commons.Enums;

namespace ShoesShop.Domain.Modules.User.Commons.Entities
{
    public abstract class EntityLastActionLog : AuditLastActionLog<int, LastAction>, IEntityLastActionLog
    {
    }

    public abstract class EntityLastActionLog<TKey> : BaseEntity<TKey>, IEntityLastActionLog<TKey>
        where TKey : struct
    {
        public int LastActionBy { get; set; }

        public LastAction LastAction { get; set; }

        public DateTime LastActionTimeStamp { get; set; }
    }
}