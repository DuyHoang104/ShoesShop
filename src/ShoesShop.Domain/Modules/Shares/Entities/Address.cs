using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Domain.Modules.Orders.Entities;

namespace ShoesShop.Domain.Modules.Shares.Entities
{
    public class Address : BaseEntity<int>
    {
        private string _addressLine1 = string.Empty;
        public string AddressLine1
        {
            get => _addressLine1;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("AddressLine1 is not empty or contains only whitespace");
                }
                if (value.Length > 100)
                {
                    throw new ArgumentException("AddressLine1 must be less than 100 characters");
                }

                _addressLine1 = value;
            }
        }
        
        private string? _city;
        public string? City
        {
            get => _city;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("City is not empty or contains only whitespace");
                    }
                    if (value.Length > 100)
                    {
                        throw new ArgumentException("City must be less than 100 characters");
                    }
                }

                _city = value;
            }
        }

        private string? _country;
        public string? Country
        {
            get => _country;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Country is not empty or contains only whitespace");
                    }
                    if (value.Length > 100)
                    {
                        throw new ArgumentException("Country must be less than 100 characters");
                    }
                }

                _country = value;
            }
        }

        public bool IsDefault { get; set; } = false;
        
        private int _userId;
        public int UserId
        {
            get => _userId;
        }

        private User _user = null!;
        public User User
        {
            get => _user;
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(User), "User cannot be null.");
                }

                _userId = value.Id;
                _user = value;
            }
        }
        
    
        private readonly HashSet<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders;

        public Address(User user,string addressLine1, string? city, string? country, bool isDefault)
        {
            User = user;
            AddressLine1 = addressLine1;
            City = city;
            Country = country;
            IsDefault = isDefault;
        }

        public Address() { }
    }
}