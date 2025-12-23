using Microsoft.Extensions.Logging;
using ShoesShop.Domain.Modules.Shares.Review.Dtos;
using ShoesShop.Domain.Modules.Shares.Review.Entity;
using ShoesShop.Domain.Modules.Shares.Review.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Users.Reviews.Services;

public class ReviewService : IReviewService
{
    private readonly IGenericRepository<Review, int> _reviewRepository;
    private readonly IGenericRepository<Order, int> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CloudinaryService _cloudinaryService;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IGenericRepository<Review, int> reviewRepository, IGenericRepository<Order, int> orderRepository, 
        IUnitOfWork unitOfWork, CloudinaryService cloudinaryService, ILogger<ReviewService> logger)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
    }

    private static bool IsValidProductReview(ProductReviewDto pr, out bool hasInput)
    {
        hasInput = false;

        var hasComment = !string.IsNullOrWhiteSpace(pr.ProductComment);
        var hasRating  = pr.ProductRating is >= 1 and <= 5;
        var hasImages  = pr.Images?.Any() == true;

        hasInput = hasComment || hasRating || hasImages;

        if (!hasInput)
            return true;

        return hasImages && (hasComment || hasRating);
    }
    public async Task CreateReviewAsync(int userId, ReviewDto dto)
    {
        var uploadedImagePublicIds = new List<string>();

        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (dto.CreateReview == null)
                throw new ArgumentException("Please provide review data");

            var order = await _orderRepository.GetAsync(
                o => o.Id == dto.CreateReview.OrderId && o.UserId == userId
            ) ?? throw new Exception("Order not found");

            var orderReview = new Review(
                rating: dto.Rating,
                comment: dto.Comment?.Trim() ?? string.Empty,
                metadata: dto.CreateReview
            );

            await _reviewRepository.InsertAsync(orderReview);

        
            if (dto.ProductReview?.Any() == true)
            {
                foreach (var pr in dto.ProductReview)
                {
                    if (!IsValidProductReview(pr, out var hasInput))
                        throw new InvalidOperationException(
                            $"Invalid product review for OrderDetailId {pr.OrderDetailId}"
                        );

                    if (!hasInput)
                        continue;

                    var uploadedImages = new List<(string Url, string PublicId)>();

                    foreach (var file in pr.Images!)
                    {
                        var image = await _cloudinaryService.UploadImageAsync(
                            file,
                            $"Reviews/Order-{order.Id}/OrderDetail-{pr.OrderDetailId}"
                        ) ?? throw new InvalidOperationException("Upload product image failed");

                        uploadedImages.Add((image.Url, image.FileName));
                        uploadedImagePublicIds.Add(image.FileName);
                    }

                    var productMetadata = new
                    {
                        pr.OrderDetailId,
                        Images = uploadedImages.Select(x => new
                        {
                            x.Url,
                            x.PublicId
                        }).ToList()
                    };

                    var productReview = new Review(
                        rating: pr.ProductRating,
                        comment: pr.ProductComment?.Trim() ?? string.Empty,
                        metadata: productMetadata
                    );

                    orderReview.AddChild(productReview);

                    foreach (var img in uploadedImages)
                    {
                        productReview.AddImage(img.Url, img.PublicId);
                    }

                    await _reviewRepository.InsertAsync(productReview);
                }
            }

            await _reviewRepository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            foreach (var publicId in uploadedImagePublicIds)
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }
                catch
                {
                    _logger.LogError($"Failed to delete image with PublicId: {publicId} during rollback.");
                }
            }

            throw;
        }
    }
}