using ShoesShop.Crosscutting.Utilities.Objects.Audit;
using ShoesShop.Domain.Modules.Commons.Enums;

namespace ShoesShop.Domain.Modules.Commons.Entities
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