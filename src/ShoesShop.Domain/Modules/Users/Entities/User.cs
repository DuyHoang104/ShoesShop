using System.Text.RegularExpressions;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Users.Enums;

namespace ShoesShop.Domain.Modules.Users.Entities
{
    public partial class User : BaseEntity<int>
    {
        private string _userName = string.Empty;
        public string UserName 
        {
            get => _userName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Username cannot be empty or whitespace.", nameof(UserName));
                }
                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(UserName), "Username cannot exceed 100 characters.");
                }

                _userName = value;
            }
        }

        private string _password = string.Empty;
        public required string Password 
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Password cannot be empty or whitespace.", nameof(Password));
                }
                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(Password), "Password cannot exceed 100 characters.");
                }
                
                _password = value;
            } 
        }

        private DateOnly _dateOfBirth;
        public DateOnly DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var minimumAgeDate = today.AddYears(-18);
                var maximumAgeDate = today.AddYears(-90);

                if (value > minimumAgeDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(DateOfBirth), $"User must be at least {minimumAgeDate} years old.");
                }

                if (value < maximumAgeDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(DateOfBirth), $"Users must be less than 90 years old.");
                }

                _dateOfBirth = value;
            }
        }

        
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(Email), "Email must be less than 100 characters.");
                }

                if (!MyRegex3().IsMatch(value))
                {
                    throw new FormatException("Invalid email format.");
                }
            
                _email = value;
            }
        }

        private string? _phone;
        public string? Phone
        {
            get => _phone;
            set
            {
                if (value != null)
                {
                    if (value.Length is < 5 or > 20)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Phone), "Phone must be between 5 and 20 characters.");
                    }

                    value = FormatPhoneNumber(value);

                    if (!MyRegex2().IsMatch(value))
                    {
                        throw new FormatException("Invalid phone number format. Please use `0xxxxxxxxxx`.");
                    }
                }

                _phone = value;
            }
        }

        private static string FormatPhoneNumber(string rawPhone)
        {
            if (string.IsNullOrWhiteSpace(rawPhone))
                throw new FormatException("Phone number is empty.");

            string phone = MyRegex().Replace(rawPhone, "");

            if (!MyRegex1().IsMatch(phone))
                throw new FormatException("Invalid Vietnamese phone number format.");

            return phone;
        }

        private UserStatus _status;
        public UserStatus Status
        {
            get => _status;
            set
            {
                if (!Enum.IsDefined(typeof(UserStatus), value))
                {
                    throw new ArgumentException("Invalid status value.", nameof(value));
                }

                _status = value;
            }
        }

        private UserAccountRole _role;
        public UserAccountRole Role
        {
            get => _role;
            set
            {
                if (!Enum.IsDefined(value))
                {
                    throw new ArgumentException("Invalid role value.", nameof(value));
                }

                _role = value;
            }
        }

        private string? _avatarUrl;
        public string? AvatarUrl
        {
            get => _avatarUrl;
            set
            {
                if (value != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Image cannot be empty or whitespace.", nameof(AvatarUrl));
                        }

                    if (value.Length > 2048)
                    {
                        throw new ArgumentOutOfRangeException(nameof(AvatarUrl), "Image cannot exceed 2048 characters.");
                        }
                    }

                _avatarUrl = value;
            }
        }   

        private UserGender _gender;
        public UserGender Gender
        {
                get => _gender;
                set 
                {
                    if (!Enum.IsDefined(value))
                    {
                        throw new ArgumentException("Invalid gender value.", nameof(value));
                    }

                    _gender = value;
                }
            }
        
        private readonly HashSet<Address> _addresses = [];
        public IReadOnlyCollection<Address> Addresses => _addresses;

        public void AddAddress(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);
            _addresses.Add(address);
        }

        public void RemoveAddress(Address address)
        {
            ArgumentNullException.ThrowIfNull(address);
            _addresses.Remove(address);
        }

        private readonly HashSet<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders;

        private readonly HashSet<CartItem> _cartItems = [];
        public IReadOnlyCollection<CartItem> CartItems => _cartItems;

        public User(string userName, string password, DateOnly dateOfBirth, string email, string? phone, string? avatarUrl, UserAccountRole role, UserStatus status, UserGender gender)
        {
            UserName = userName;
            Password = password;
            DateOfBirth = dateOfBirth;
            Email = email;
            AvatarUrl = avatarUrl;
            Phone = phone;
            Role = role;
            Gender = gender;
        }

        public User() { }

        [GeneratedRegex(@"\s|-")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"^0\d{9,10}$")]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"^0\d{9,10}$")]
        private static partial Regex MyRegex2();
        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex MyRegex3();
    }
}