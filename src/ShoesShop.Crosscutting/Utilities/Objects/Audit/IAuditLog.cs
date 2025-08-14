namespace ShoesShop.Crosscutting.Utilities.Objects.Audit
{
    public interface IAuditLog<TKey, TAction> : IAuditCreatedLog<TKey>, IAuditLastActionLog<TKey, TAction> where TKey : struct where TAction : struct { }
}