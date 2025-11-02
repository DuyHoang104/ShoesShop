using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Products.Enums;
using ShoesShop.Domain.Modules.Shares.Entities;

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

    private string _brand = string.Empty;
    public string Brand
    {
        get => _brand;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Brand cannot be empty or whitespace.", nameof(Brand));
            }

            if (value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Brand), "Brand cannot exceed 100 characters.");
            }

            _brand = value;
        }
    }

    private string _color = string.Empty;
    public string Color
    {
        get => _color;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Color cannot be empty or whitespace.", nameof(Color));
            }

            if (value.Length > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(Color), "Color cannot exceed 50 characters.");
            }

            _color = value;
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

    private string _sizes = string.Empty;
    public string Sizes
    {
        get => _sizes;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Sizes cannot be empty or whitespace.", nameof(Sizes));
            }

            _sizes = value;
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

    private ProductStatus? _status;
    public ProductStatus? Status
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

    private readonly HashSet<ImageUpload> _images = new();
    public IReadOnlyCollection<ImageUpload> Images => _images;

    public void AddImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be empty.", nameof(url));

        if (_images.Any(i => i.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Image URL '{url}' already exists for this product.");

        _images.Add(new ImageUpload(url, this));
    }

    public void RemoveImage(string url)
    {
        var image = _images.FirstOrDefault(i => i.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Image not found.");
        _images.Remove(image);
    }
    
    private readonly HashSet<ProductCategory> _productCategories = [];
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories;
    
    public void AddCategory(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");
        }

        if (_productCategories.Any(pc => 
                pc.Category.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Category '{category.Name}' already exists for this product.");
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
    
    public Product(string name, string description, decimal price, string brand, string color, int quantity, decimal? saleOff, ProductStatus? status, string sizes)
    {
        Name = name;
        Description = description;
        Price = price;
        Brand = brand;
        Color = color;
        Quantity = quantity;
        SaleOff = saleOff;
        Sizes = sizes;
        Status = status;
    }

    public Product() { }
}