namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public class AuditCreateLog<TKey> : IAuditCreatedLog<TKey> where TKey : struct
    {
        public virtual TKey CreateBy { get; set; }

        public virtual DateTime CreateTimeStamp { get; set; }
    }
}