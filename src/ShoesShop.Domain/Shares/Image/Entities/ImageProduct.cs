using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Shares.Image.Enums;

namespace ShoesShop.Domain.Shares.Image.Entities;

public class ImageProduct : Image
{
    public Product? Product { get; set; }
    
    public ImageProduct() : base()
    {
    }

    public ImageProduct(string url, string publicId) : base()
    {
        Url = url;
        PublicId = publicId;
        OwnerType = OwnerType.Product;
    }
}