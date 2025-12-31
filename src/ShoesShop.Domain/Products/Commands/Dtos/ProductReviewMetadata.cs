using System.Text.Json.Serialization;

namespace ShoesShop.Domain.Products.Commands.Dtos;

public class ProductReviewMetadata
{
    [JsonPropertyName("orderDetailId")]
    public int OrderDetailId { get; set; }

    [JsonPropertyName("productId")]
    public int ProductId { get; set; }
    

     [JsonPropertyName("userId")]
    public int UserId { get; set; }


    [JsonPropertyName("images")]
    public List<ReviewImageMeta>? Images { get; set; }
}

public class ReviewImageMeta
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("publicId")]
    public string PublicId { get; set; } = string.Empty;
}
