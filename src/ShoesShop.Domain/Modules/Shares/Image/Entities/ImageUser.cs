using ShoesShop.Domain.Modules.Shares.Image.Enums;

namespace ShoesShop.Domain.Modules.Shares.Image.Entities
{
    public class ImageUser : Image
    {
        public User.Users.Entities.User? User { get; set; }
        
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
}