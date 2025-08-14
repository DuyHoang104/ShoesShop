namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public interface IAuditCreatedLog<TKey> where TKey : struct
    {
        public TKey CreateBy { get; set; }

        public DateTime CreateTimeStamp { get; set; }
    }
}