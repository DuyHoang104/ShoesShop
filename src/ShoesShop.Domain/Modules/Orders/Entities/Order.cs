using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Orders.Enums;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Entities;

namespace ShoesShop.Domain.Modules.Orders.Entities;

public class Order : BaseEntity<int>
{
    private DateTime _orderDate = DateTime.UtcNow;
    public DateTime OrderDate
    {
        get => _orderDate;
        set
        {
            _orderDate = value;
        }
    }

    private OrderStatus _status = OrderStatus.Pending;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(typeof(OrderStatus), value))
            {
                throw new ArgumentException("Invalid status value.", nameof(value));
            }

            _status = value;
        }
    }

    public decimal TotalAmount =>
    OrderDetails.Sum(d => d.UnitPrice * d.Quantity)
    + ShippingFee.GetValueOrDefault()
    - Discount.GetValueOrDefault();

    private PaymentMethod _paymentMethod;
    public PaymentMethod PaymentMethod
    {
        get => _paymentMethod;
        set
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), value))
            {
                throw new ArgumentException("Invalid payment method value.", nameof(value));
            }

            _paymentMethod = value;
        }
    }

    private PaymentStatus _paymentStatus;
    public PaymentStatus PaymentStatus
    {
        get => _paymentStatus;
        set
        {
            if (!Enum.IsDefined(typeof(PaymentStatus), value))
            {
                throw new ArgumentException("Invalid payment status value.", nameof(value));
            }

            _paymentStatus = value;
        }
    }

    private string? _note;
    public string? Note
    {
        get => _note;
        set
        {
            if (value != null)
            {
                if (value.Length > 255)
                {
                    throw new ArgumentException("Note cannot be longer than 255 characters.", nameof(value));
                }
            }

            _note = value;
        }
    }

    private string? _receiverName = null!;
    public string? ReceiverName
    {
        get => _receiverName;
        set
        {
            if (value != null)
            {
                if (value.Length > 100)
                {
                    throw new ArgumentException("Receiver's name cannot be longer than 100 characters.", nameof(value));
                }
            }

            _receiverName = value;
        }
    }

    private string? _receiverPhone = null!;
    public string? ReceiverPhone
    {
        get => _receiverPhone;
        set
        {
            if (value != null)
            {
                if (value.Length > 15)
                {
                    throw new ArgumentException("Receiver's phone number cannot be longer than 15 characters.", nameof(value));
                }
            }

            _receiverPhone = value;
        }
    }

    private string? _receiverAddress;
    public string? ReceiverAddress
    {
        get => _receiverAddress;
        set
        {
            if (value != null && value.Length > 255)
                throw new ArgumentException("Receiver's address cannot be longer than 255 characters.", nameof(value));
            _receiverAddress = value;
        }
    }
    
    private decimal? _shippingFee;
    public decimal? ShippingFee
    {
        get => _shippingFee;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ShippingFee), "Shipping fee cannot be negative.");
            }

            _shippingFee = value;
        }
    }

    private decimal? _discount;
    public decimal? Discount
    {
        get => _discount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Discount), "Discount cannot be negative.");
            }

            _discount = value;
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

    private readonly HashSet<OrderDetail> _orderDetails = new();
    public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;
    public void AddOrderDetail(OrderDetail orderDetail)
    {
        if (orderDetail == null)
        {
            throw new ArgumentNullException(nameof(orderDetail), "Order detail cannot be null.");
        }

        _orderDetails.Add(orderDetail);
    }

    public Order(User user, Address address, PaymentMethod paymentMethod, PaymentStatus paymentStatus, string? receiverName,
        string? receiverPhone, string? receiverAddress, string? note = null, decimal? shippingFee = null, decimal? discount = null)
    {
        User = user;
        Address = address;
        ReceiverName = receiverName;
        ReceiverPhone = receiverPhone;
        ReceiverAddress = receiverAddress;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
        ShippingFee = shippingFee;
        Discount = discount;
        Note = note;
        Status = OrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
    }

    public Order() { }
}