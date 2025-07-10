using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Crosscutting.Utilities.Objects.ObjectId
{
    public class ObjectId<TKey> : IObjectId<TKey> where TKey : struct
    {
        [Key]
        public virtual TKey Id { get; set; }

        private int HashCodeMultiplier;

        private int? _requestedHashCode;

        public ObjectId()
        {
            HashCodeMultiplier = 1;
        }

        public ObjectId(int hashCodeMultiplier)
        {
            HashCodeMultiplier = hashCodeMultiplier;
        }

        public bool IsTransient() => Equals(Id, default(TKey));

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
                
            if (obj is not ObjectId<TKey>)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            ObjectId<TKey> item = (ObjectId<TKey>)obj;

            if (item.IsTransient() || IsTransient())
                return false;

            return Equals(item.Id, Id);
        }

        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();
                
            if (!_requestedHashCode.HasValue)
                    _requestedHashCode = Id.GetHashCode() ^ HashCodeMultiplier;

                return _requestedHashCode.Value;
        }
    }
}