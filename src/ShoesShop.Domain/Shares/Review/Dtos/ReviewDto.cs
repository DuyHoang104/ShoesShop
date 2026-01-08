using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShoesShop.Domain.Shares.Review.Enums;
using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Domain.Shares.Review.Dtos;

public class ReviewDto
{
    public int Id { get; set; }

    public string Comment { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int Rating { get; set; }

    public object? Metadata { get; set; }

    public int? ParentId { get; set; }

    public CreateReviewDto CreateReview { get; set; } = new CreateReviewDto();
    public List<ProductReviewDto>? ProductReview { get; set; } = [];
    public ReviewStatus Status { get; set; }
    public UserDto?  User { get; set;  }
}

public class CreateReviewDto
{
    [Required]
    public int OrderId { get; set; }

    [Range(1, 5)]
    public int ShipperRating { get; set; }

    [Range(1, 5)]
    public int EmployeeRating { get; set; }
}

public class ProductReviewDto
{
    public List<IFormFile>? Images { get; set; } = [];
    public int? ImageId { get; set; }
    public int OrderDetailId { get; set; }
    public string? ProductComment { get; set; }
    public int ProductId { get; set; }
    [Range(1, 5)]
    public int ProductRating { get; set; }
    public List<string>? ImageUrls { get; set; }
    public int UserId { get; set; }
}