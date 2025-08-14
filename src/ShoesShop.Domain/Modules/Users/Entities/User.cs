using System.Text.RegularExpressions;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Users.Enums;

namespace ShoesShop.Domain.Modules.Users.Entities
{
    public class User : BaseEntity<int>
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

                if (!Regex.IsMatch(
                        value,
                        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                        RegexOptions.IgnoreCase))
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

                    if (!Regex.IsMatch(value, @"^\+\d{1,3}-\d{3,4}-\d{3}-\d{3}$"))
                    {
                        throw new FormatException("Invalid phone number format. Please use +(country_code)-xxx-xxx-xxx.");
                    }
                }

                _phone = value;
            }
        }

        private static string FormatPhoneNumber(string phone)
        {
            phone = Regex.Replace(phone, @"\s|-", "");

            string countryCode = "+84"; 

            var match = Regex.Match(phone, @"^\+(\d{1,3})(\d+)$");
            if (match.Success)
            {
                countryCode = "+" + match.Groups[1].Value;
                phone = match.Groups[2].Value;
            }
            else if (phone.StartsWith("0"))
            {
                phone = phone.Substring(1);
            }
            else
            {
                throw new FormatException("Invalid phone number. Please include a valid country code or start with 0.");
            }

            return Regex.Replace(countryCode + phone, @"(\+\d{1,3})(\d{3,4})(\d{3})(\d{3})", "$1-$2-$3-$4");
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

        private UserGender _gender; 
        public UserGender Gender 
        { 
            get => _gender;
            set 
            {
                if (!Enum.IsDefined(typeof(UserGender), value))
                {
                    throw new ArgumentException("Invalid gender value.", nameof(value));
                }

                _gender = value;
            }
        }
        
        private readonly HashSet<Address> _addresses = [];
        public IReadOnlyCollection<Address> Addresses => _addresses;

        private readonly HashSet<Order> _orders = [];
        public IReadOnlyCollection<Order> Orders => _orders;

        private readonly HashSet<CartItem> _cartItems = [];
        public IReadOnlyCollection<CartItem> CartItems => _cartItems;

        public User(string userName, string password, DateOnly dateOfBirth, string email, string? phone, UserStatus status, UserGender gender)
        {
            UserName = userName;
            Password = password;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Status = status;
            Gender = gender;
        }

        public User() { }
    }
}