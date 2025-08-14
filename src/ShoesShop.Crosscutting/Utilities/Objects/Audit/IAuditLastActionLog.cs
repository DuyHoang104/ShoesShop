namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public interface IAuditLastActionLog<TKey, TAction> where TKey : struct where TAction : struct
    {
        public TKey LastActionBy { get; set; }

        public TAction LastAction { get; set; }

        public DateTime LastActionTimeStamp { get; set; }
    }
}