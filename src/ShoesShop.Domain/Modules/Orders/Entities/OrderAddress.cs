using ShoesShop.Domain.Modules.Addresses.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;

namespace ShoesShop.Domain.Modules.Orders.Entities
{
    public class OrderAddress : BaseEntity<int>
    {
        private int _orderId;
        public int OrderId
        {
            get => _orderId;
        }

        private Order _order = null!;
        public Order Order
        {
            get => _order;
            internal set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Order), "Order cannot be null.");
                }
                
                _orderId = value.Id;
                _order = value;
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

        public OrderAddress(Order order, Address address)
        {
            Order = order;
            Address = address;
        }

        public OrderAddress() { }
    }
} 