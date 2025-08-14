using ShoesShop.Crosscutting.Utilities.Objects.Audit;

namespace ShoesShop.Domain.Modules.Commons.Entities
{
    public abstract class EntityCreatedLog : AuditCreateLog<int>, IEntityCreatedLog
    {
    }

    public abstract class EntityCreatedLog<TKey> : BaseEntity<TKey>, IEntityCreatedLog<TKey>
        where TKey : struct
    {
        public int CreateBy { get; set; }

        public DateTime CreateTimeStamp { get; set; }
    }
}