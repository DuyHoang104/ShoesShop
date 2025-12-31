using ShoesShop.Domain.Shares.Image.Enums;

namespace ShoesShop.Domain.Shares.Image.Entities;

public class ImageReview : Image
{
    public Review.Entity.Review? Review { get; set; }
    
    public ImageReview() : base()
    {
    }
    
    public ImageReview(string url, string publicId) : base()
    {
        Url = url;
        PublicId = publicId;
        OwnerType = OwnerType.Review;
    }
}