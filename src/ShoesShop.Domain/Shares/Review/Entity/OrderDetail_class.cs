namespace ShoesShop.Domain.Shares.Review.Entity;

public class OrderDetail_class
{
    public int? ProductRating { get; private set; }
    public int? ImageId { get; private set; }
    public int OrderDetailId { get; private set; }

    protected OrderDetail_class() { }

    public OrderDetail_class(int? productRating, int orderDetailId, int? imageId = null)
    {
        ProductRating = productRating;
        OrderDetailId = orderDetailId;
        ImageId = imageId;
    }
}