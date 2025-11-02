using ShoesShop.Domain.Modules.Commons.Entities;

namespace ShoesShop.Domain.Modules.Products.Entities;

public class ImageUpload : EntityAuditLog<int>
{
    private string _url = string.Empty;

    public string Url
    {
        get => _url;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Image URL cannot be empty or whitespace.", nameof(Url));

            if (!value.StartsWith("/") && !value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Image URL must start with '/' or 'http'.", nameof(Url));

            if (value.Length > 2048)
                throw new ArgumentOutOfRangeException(nameof(Url), "Image URL cannot exceed 2048 characters.");

            _url = value;
        }
    }

    private int _productId;
    public int ProductId
    {
        get => _productId;
        private set
        {
            _productId = value;
        }
    }

    private Product? _product;
    public Product Product
    {
        get => _product ?? throw new InvalidOperationException("Product reference not loaded.");
        private set => _product = value ?? throw new ArgumentNullException(nameof(Product), "Product cannot be null.");
    }

    public ImageUpload(string url, Product product)
    {
        Url = url;
        Product = product ?? throw new ArgumentNullException(nameof(product));
        ProductId = product.Id;
    }

    protected ImageUpload() { }
}
