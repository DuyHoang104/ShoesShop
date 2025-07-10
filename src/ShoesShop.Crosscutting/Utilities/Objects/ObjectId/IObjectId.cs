namespace ShoesShop.Crosscutting.Utilities.Objects.ObjectId
{
    public interface IObjectId<TKey> where TKey : struct
    {
        public TKey Id { get; set; }
    }
}