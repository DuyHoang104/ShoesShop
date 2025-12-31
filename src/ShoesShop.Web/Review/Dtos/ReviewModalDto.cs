using System.ComponentModel.DataAnnotations;
using ShoesShop.Domain.Products.Commands;

namespace ShoesShop.Web.Review.Dtos;

public class ReviewModalDto
{
    public int Id { get; set; }

    public string Comment { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    public object? Metadata { get; set; }

    public int? ParentId { get; set; }

    public CreateReviewModalDto? CreateReview { get; set; } = new CreateReviewModalDto();
    public List<ProductReviewModalDto>? ProductReviews { get; set; } = [];
}

public class CreateReviewModalDto
{
    [Required]
    public int OrderId { get; set; }

    [Range(1, 5)]
    public int ShipperRating { get; set; }

    [Range(1, 5)]
    public int EmployeeRating { get; set; }
}

public class ProductReviewModalDto
{
    public List<IFormFile> Images { get; set; } = [];
    public int? ImageId { get; set; }
    public int OrderDetailId { get; set; }
    public string? ProductComment { get; set; }
    public ProductDto? Product { get; set; }

    [Range(1, 5)]
    public int ProductRating { get; set; }
    public int ProductId { get; set; }
}