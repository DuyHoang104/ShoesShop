using ShoesShop.Domain.Modules.Shares.Image.Enums;
using ShoesShop.Domain.Modules.User.Products.Entities;

namespace ShoesShop.Domain.Modules.Shares.Image.Entities
{
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
}