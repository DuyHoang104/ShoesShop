using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Orders.Services;
using ShoesShop.Domain.Shares.Image.Dtos;
using ShoesShop.Domain.Shares.Review.Dtos;
using ShoesShop.Domain.Shares.Review.Services;
using ShoesShop.Web.Review.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace ShoesShop.Web.Review.Controllers.User;

[Route("User/Review")]
[Authorize(AuthenticationSchemes = "UserScheme", Roles = "Customer")]
public class ReviewController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IReviewService _reviewService;

    public ReviewController(
        IOrderService orderService,
        IReviewService reviewService)
    {
        _orderService = orderService;
        _reviewService = reviewService;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst("UserId")!.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int orderId)
    {
        var order = await _orderService.GetOrderDetailByIdAsync(orderId);
        if (order == null)
            return NotFound();

        var model = new ReviewModalDto
        {
            CreateReview = new CreateReviewModalDto
            {
                OrderId = order.Id
            },

            ProductReviews = order.OrderDetails.Select(od => new ProductReviewModalDto
            {
                OrderDetailId = od.Id,
                ProductId = od.ProductId,
                Product = new ProductDto
                {
                    Name = od.ProductName ?? string.Empty,
                    Price = od.UnitPrice,
                    Quantity = od.Quantity,
                    Sizes = od.Size ?? string.Empty,
                    Images = od.ProductImage != null ?
                    [
                        new ImageDto
                        {
                            Url = od.ProductImage
                        }
                    ] : []
                },
            }).ToList()
        };

        return View("~/Review/Views/User/CreateReview.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] ReviewModalDto model)
    {
        var userId = GetCurrentUserId();

        var reviewDto = new ReviewDto
        {
            Comment = model.Comment,
            Rating = model.Rating,
            ParentId = null,
            Metadata = model.CreateReview,

            CreateReview = new CreateReviewDto
            {
                OrderId = model.CreateReview!.OrderId,
                ShipperRating = model.CreateReview.ShipperRating,
                EmployeeRating = model.CreateReview.EmployeeRating
            },

            ProductReview = model.ProductReviews?
                .Where(pr =>
                    pr.OrderDetailId > 0 &&
                    (
                        pr.ProductRating > 0 ||
                        !string.IsNullOrWhiteSpace(pr.ProductComment) ||
                        (pr.Images != null && pr.Images.Any())
                    )
                )
                .Select(pr => new ProductReviewDto
                {
                    OrderDetailId = pr.OrderDetailId,
                    ProductRating = pr.ProductRating,
                    ProductComment = pr.ProductComment,
                    ProductId = pr.ProductId,
                    Images = pr.Images,
                    ImageId = pr.ImageId
                })
                .ToList()
                ?? []
        };

        await _reviewService.CreateReviewAsync(userId, reviewDto);
        return RedirectToAction("Index", "Order");
    }
}