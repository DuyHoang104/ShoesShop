using ShoesShop.Crosscutting.Utilities.Objects.Audit;

namespace ShoesShop.Domain.Modules.Commons.Entities
{
    public interface IEntityCreatedLog : IBaseEntity, IAuditCreatedLog<int>
    {
    }

    public interface IEntityCreatedLog<TKey> : IBaseEntity<TKey>, IEntityCreatedLog 
        where TKey : struct
    {
    }
}