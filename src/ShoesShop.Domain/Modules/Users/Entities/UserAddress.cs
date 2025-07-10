using ShoesShop.Domain.Modules.Addresses.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;

namespace ShoesShop.Domain.Modules.Users.Entities
{
    public class UserAddress : EntityAuditLog<int>
    {
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

        private int _addressId;
        public int AddressId
        {
            get => _addressId;
        }

        private Address _address = null!;
        public Address Address
        {
            get => _address;
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Address), "Address cannot be null.");
                }
                
                _addressId = value.Id;
                _address = value;
            }
        }

        public UserAddress(User user, Address address)
        {
            User = user;
            Address = address;
        }

        public UserAddress() { }
    }
}