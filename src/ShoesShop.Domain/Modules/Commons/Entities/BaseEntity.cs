using ShoesShop.Crosscutting.Utilities.Objects.ObjectId;

namespace ShoesShop.Domain.Modules.Commons.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
    }

    public abstract class BaseEntity<TKey> : ObjectId<TKey>, IBaseEntity<TKey>
        where TKey : struct
    {
        protected BaseEntity() : base(35)
        {
        }
    }
}