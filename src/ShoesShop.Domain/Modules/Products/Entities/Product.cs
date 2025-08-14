using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Products.Enums;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Carts.Entities;

namespace ShoesShop.Domain.Modules.Products.Entities;

public class Product : EntityAuditLog<int>
{
    private string _name = string.Empty;
    public string Name 
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be empty or whitespace.", nameof(Name));
            }
            if (value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Name), "Name cannot exceed 100 characters.");
            }

            _name = value;
        }
    }

    private string _description = string.Empty;
    public string Description 
    {
        get => _description;
        set
        {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Description cannot be empty or whitespace.", nameof(Description));
                }

                if (value.Length > 500)
                {
                    throw new ArgumentOutOfRangeException(nameof(Description), "Description cannot exceed 500 characters.");
                }

            _description = value;
        }
    }

    private decimal _price;
    public decimal Price 
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Price), "Price cannot be negative.");
            }

            _price = value;
        }
    }

    private int _quantity;
    public int Quantity 
    {
        get => _quantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity cannot be negative.");
            }
        
            _quantity = value;
        }
    }

    private string? _imageUrl;
    public string? ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Image cannot be empty or whitespace.", nameof(ImageUrl));
                }

                if (value.Length > 2048)
                {
                    throw new ArgumentOutOfRangeException(nameof(ImageUrl), "Image cannot exceed 2048 characters.");
                }
            }

            _imageUrl = value;
        }
    }
    
    private decimal? _saleOff;
    public decimal? SaleOff
    {
        get => _saleOff;
        set
        {
            if (value != null)
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(SaleOff), "SaleOff cannot be negative.");
                }
            }

            _saleOff = value;
        }
    }

    private ProductStatus _status;
    public ProductStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(typeof(ProductStatus), value))
            {
                throw new ArgumentException("Invalid status value.", nameof(value));
            }

            _status = value;
        }
    }

    private readonly HashSet<ProductCategory> _productCategories = [];
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories;
    
    public void AddCategory(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");
        }
        if (_productCategories.Any(pc => pc.CategoryId == category.Id))
        {
            throw new InvalidOperationException("Product already has this category.");
        }

        _productCategories.Add(new ProductCategory(this, category));
    }

    public void RemoveCategory(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");
        }

        var productCategory = _productCategories.FirstOrDefault(x => x.Category.Equals(category)) 
            ?? throw new InvalidOperationException("Category not found.");
        
        _productCategories.Remove(productCategory);
    }

    private readonly HashSet<OrderDetail> _orderDetails = [];
    public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;

    private readonly HashSet<CartItem> _cartItems = [];
    public IReadOnlyCollection<CartItem> CartItems => _cartItems;

    public Product(string name, string description, decimal price, int quantity, string? imageUrl, decimal? saleOff, ProductStatus status)
    {
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
        ImageUrl = imageUrl;
        SaleOff = saleOff;
        Status = status;
    }

    public Product() { }
}