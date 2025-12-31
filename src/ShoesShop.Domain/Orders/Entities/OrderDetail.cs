using ShoesShop.Domain.Commons.Entities;
using ShoesShop.Domain.Products.Entities;

namespace ShoesShop.Domain.Orders.Entities;

public class OrderDetail : BaseEntity<int>
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

    private int _productId;
    public int ProductId
    {
        get => _productId;
    }

    private Product _product = null!;
    public Product Product
    {
        get => _product;
        internal set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(Product), "Product cannot be null.");
            }
            
            _productId = value.Id;
            _product = value;
        }
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0) 
            {
                throw new ArgumentException("Quantity must be greater than 0.", nameof(value));
            }

            _quantity = value;
        }
    }

    private decimal _unitPrice;
    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Unit price must be greater than 0.", nameof(value));
            }

            _unitPrice = value;
        }
    }

    private decimal _subtotal;
    public decimal Subtotal
    {
        get => _subtotal;
        private set
        {
            if (value < 0)
            {
                throw new ArgumentException("Subtotal must be greater than 0.", nameof(value));
            }

            _subtotal = value;
        }
    }

    private string _size = string.Empty;
    public string Size
    {
        get => _size;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Size cannot be null or empty.", nameof(value));
            }

            _size = value;
        }
    }

    public OrderDetail(Order order, Product product, int quantity, decimal subtotal, string size)
    {
        Order = order;
        Product = product;
        Quantity = quantity;
        UnitPrice = product.Price;
        Subtotal = subtotal;
        Size = size;
    }

    public OrderDetail() { }
}