using ShoesShop.Crosscutting.Utilities.Objects.ObjectId;

namespace ShoesShop.Domain.Modules.Commons.Entities
{
    public interface IBaseEntity
    {
    }

    public interface IBaseEntity<TKey> : IObjectId<TKey>, IBaseEntity
        where TKey : struct
    {
    }
}