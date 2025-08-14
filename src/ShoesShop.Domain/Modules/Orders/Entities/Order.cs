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
            if (value < DateTime.UtcNow)
            {
                throw new ArgumentException("Order date cannot be in the past.", nameof(value));
            }
            
            _orderDate = value;
        }
    }

    private OrderStatus _status;
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

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => _totalAmount;
        private set
        {
            if (value < 0)  
            {
                throw new ArgumentOutOfRangeException(nameof(TotalAmount), "Total amount cannot be negative.");
            }

            _totalAmount = value;
        }
    }

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

    private readonly HashSet<OrderDetail> _orderDetails = [];
    public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;

    public Order(User user, Address address, decimal totalAmount, PaymentMethod paymentMethod, PaymentStatus paymentStatus, string? note)
    {
        User = user;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
        Address = address;
        Note = note;
    }

    public Order() { }
}