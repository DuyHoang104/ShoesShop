namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public class AuditLog<TKey, TAction> : IAuditLog<TKey, TAction> where TKey : struct where TAction : struct
    {
        public virtual TKey CreateBy { get; set; }

        public virtual DateTime CreateTimeStamp { get; set; }

        public virtual TKey LastActionBy { get; set; }

        public virtual TAction LastAction { get; set; }

        public virtual DateTime LastActionTimeStamp { get; set; }
    }

}