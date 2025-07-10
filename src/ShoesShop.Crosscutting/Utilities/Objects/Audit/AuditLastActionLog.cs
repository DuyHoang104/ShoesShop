namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public class AuditLastActionLog<TKey, TAction> : IAuditLastActionLog<TKey, TAction> where TKey : struct where TAction : struct
    {
        public virtual TKey LastActionBy { get; set; }

        public virtual TAction LastAction { get; set; }

        public virtual DateTime LastActionTimeStamp { get; set; }
    }
}