using ShoesShop.Domain.Shares.Image.Enums;
using ShoesShop.Domain.Users.Entities;

namespace ShoesShop.Domain.Shares.Image.Entities;

public class ImageUser : Image
{
    public User? User { get; set; }
    
    public ImageUser() : base()
    {
    }
    
    public ImageUser(string url, string publicId) : base()
    {
        Url = url;
        PublicId = publicId;
        OwnerType = OwnerType.User;
    }
}