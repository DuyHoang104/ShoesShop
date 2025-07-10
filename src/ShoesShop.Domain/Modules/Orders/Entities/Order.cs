using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Orders.Enums;
using ShoesShop.Domain.Modules.Addresses.Entities;
using ShoesShop.Domain.Modules.Users.Entities;

namespace ShoesShop.Domain.Modules.Orders.Entities;

public class Order : EntityAuditLog<int>
{
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

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

    private readonly HashSet<OrderAddress> _orderAddresses = [];
    public IReadOnlyCollection<OrderAddress> OrderAddresses => _orderAddresses;
    
    public void AddAddress(Address addresses)
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses), "Order address cannot be null.");
        }
        _orderAddresses.Add(new OrderAddress(this, addresses));
    }

    public void RemoveAddress(Address address)
    {
        var orderAddress = _orderAddresses.FirstOrDefault(x => x.Address.Equals(address)) 
            ?? throw new InvalidOperationException("Order address not found.");
        
        _orderAddresses.Remove(orderAddress);
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

    public void AddOrderDetail(OrderDetail detail)
    {
        if (detail == null) throw new ArgumentNullException(nameof(detail));
        if (detail.OrderId != 0 && detail.OrderId != Id)
        {
            throw new InvalidOperationException("Chi tiết đơn hàng đã thuộc về một đơn hàng khác.");
        }
        _orderDetails.Add(detail);
        detail.Order = this;
        RecalculateTotalAmount();
    }
    
    public void RemoveOrderDetail(OrderDetail detail)
    {
        if (detail == null) throw new ArgumentNullException(nameof(detail));
        if (!_orderDetails.Remove(detail))
        {
            throw new InvalidOperationException("Không tìm thấy chi tiết đơn hàng để xóa.");
        }
        RecalculateTotalAmount();
    }

    private void RecalculateTotalAmount()
    {
        TotalAmount = OrderDetails.Sum(od => od.Subtotal);
    }

    public Order(User user, decimal totalAmount, PaymentMethod paymentMethod, PaymentStatus paymentStatus)
    {
        User = user;
        TotalAmount = totalAmount;
        PaymentMethod = paymentMethod;
        PaymentStatus = paymentStatus;
    }

    public Order() { }
}