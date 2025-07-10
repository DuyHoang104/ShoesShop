using System.Text.RegularExpressions;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Domain.Modules.Orders.Entities;

namespace ShoesShop.Domain.Modules.Addresses.Entities
{
    public class Address : EntityAuditLog<int>
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
        
        private string? _addressLine2;
        public string? AddressLine2
        {
            get => _addressLine2;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("AddressLine2 is not empty or contains only whitespace");
                    }
                    if (value.Length > 100)
                    {
                        throw new ArgumentException("AddressLine2 must be less than 100 characters");
                    }
                }

                _addressLine2 = value;
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

        private string? _state;
        public string? State
        {
            get => _state;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("State is not empty or contains only whitespace");
                    }
                    if (value.Length > 100)
                    {
                        throw new ArgumentException("State must be less than 100 characters");
                    }
                }

                _state = value;
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

        private string? _zipCode;
        public string? ZipCode
        {
            get => _zipCode;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("ZipCode is not empty or contains only whitespace");
                    }
                    if (value.Length > 10)
                    {
                        throw new ArgumentException("ZipCode must be less than 10 characters");
                    }
                    if(value.Length < 5)
                    {
                        throw new ArgumentException("ZipCode must be at least 5 characters");
                    }
                    if (!Regex.IsMatch(value, @"^\d{4,11}$"))
                    {
                        throw new ArgumentException("Invalid ZipCode format. Expected format: 4 to 11 digits.");
                    }
                }
                _zipCode = value;
            }
        }

        private readonly HashSet<OrderAddress> _orderAddresses = [];
        public IReadOnlyCollection<OrderAddress> OrderAddresses => _orderAddresses;

        private readonly HashSet<UserAddress> _userAddresses = [];
        public IReadOnlyCollection<UserAddress> UserAddresses => _userAddresses;

        public Address(string addressLine1, string? addressLine2, string? city, string? state, string? country, string? zipCode)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

        public Address() { }
    }
}