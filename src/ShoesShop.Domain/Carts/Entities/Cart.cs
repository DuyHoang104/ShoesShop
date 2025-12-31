using ShoesShop.Domain.Commons.Entities;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Users.Entities;

namespace ShoesShop.Domain.Carts.Entities;

public class Cart : BaseEntity<int>
{
    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity must be greater than 0.");
            }

            _quantity = value;
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
    
    private string _size = string.Empty;
    public string Size
    {
        get => _size;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Size cannot be null or empty.", nameof(Size));
            }

            _size = value;
        }
    }

    public Cart(User user, Product product, int quantity, string size)
    {
        User = user;
        Product = product;
        Quantity = quantity;
        Size = size;
    }
    
    public Cart() { }    
}