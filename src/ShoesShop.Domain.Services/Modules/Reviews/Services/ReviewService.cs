using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoesShop.Domain.Commons.Enums;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Orders.Entities;
using ShoesShop.Domain.Products.Commands.Dtos;
using ShoesShop.Domain.Shares.Review.Dtos;
using ShoesShop.Domain.Shares.Review.Entity;
using ShoesShop.Domain.Shares.Review.Enums;
using ShoesShop.Domain.Shares.Review.Services;
using ShoesShop.Domain.Users.Dtos;
using ShoesShop.Infrastructure.Data.UOW;
using Microsoft.AspNetCore.Http;

namespace ShoesShop.Domain.Services.Modules.Reviews.Services;

public class ReviewService : IReviewService
{
    private readonly IGenericRepository<Review, int> _reviewRepository;
    private readonly IGenericRepository<Order, int> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CloudinaryService _cloudinaryService;
    private readonly ILogger<ReviewService> _logger;
    private readonly IGenericRepository<Domain.Users.Entities.User, int> _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReviewService(
        IGenericRepository<Review, int> reviewRepository,
        IGenericRepository<Order, int> orderRepository,
        IGenericRepository<Domain.Users.Entities.User, int> userRepository,
        IUnitOfWork unitOfWork,
        CloudinaryService cloudinaryService,
        ILogger<ReviewService> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetCurrentAdminId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user == null || !user.Identity!.IsAuthenticated)
            return 0;

        var adminIdClaim =
            user.FindFirst(ClaimTypes.NameIdentifier) ??
            user.FindFirst("adminId");

        return adminIdClaim != null && int.TryParse(adminIdClaim.Value, out var id)
            ? id
            : 0;
    }

    private static bool IsValidProductReview(ProductReviewDto pr, out bool hasInput)
    {
        var hasComment = !string.IsNullOrWhiteSpace(pr.ProductComment);
        var hasRating  = pr.ProductRating is >= 1 and <= 5;
        var hasImages  = pr.Images?.Any() == true;

        hasInput = hasComment || hasImages || hasRating;

        if (hasRating && (hasComment || hasImages))
            return true;

        if (!hasInput)
            return true;

        return false;
    }

    public async Task CreateReviewAsync(int userId, ReviewDto dto)
    {
        var uploadedImagePublicIds = new List<string>();
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var order = await _orderRepository.GetAsync(
                o => o.Id == dto.CreateReview.OrderId && o.UserId == userId
            ) ?? throw new Exception("Order not found");

            var now = DateTime.UtcNow;

            var orderReview = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment?.Trim() ?? string.Empty,
                Metadata = dto.CreateReview,
                CreateBy = userId,
                CreateTimeStamp = now,
                LastActionBy = userId,
                LastAction = LastAction.Create,
                LastActionTimeStamp = now,
                Status = ReviewStatus.Active
            };

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

                    if (pr.Images?.Any() == true)
                    {
                        foreach (var file in pr.Images)
                        {
                            var image = await _cloudinaryService.UploadImageAsync(
                                file,
                                $"Reviews/Order-{order.Id}/OrderDetail-{pr.OrderDetailId}"
                            ) ?? throw new InvalidOperationException("Upload product image failed");

                            uploadedImages.Add((image.Url, image.FileName));
                            uploadedImagePublicIds.Add(image.FileName);
                        }
                    }

                    var productReview = new Review
                    {
                        Rating = pr.ProductRating,
                        Comment = pr.ProductComment?.Trim() ?? string.Empty,
                        Metadata = new
                        {
                            pr.OrderDetailId,
                            pr.ProductId,
                            userId,
                            Images = uploadedImages.Select(i => new
                            {
                                i.Url,
                                i.PublicId
                            }).ToList()
                        },
                        CreateBy = userId,
                        CreateTimeStamp = now,
                        LastActionBy = userId,
                        LastAction = LastAction.Create,
                        Status = ReviewStatus.Active,
                        LastActionTimeStamp = now
                    };

                    orderReview.AddChild(productReview);

                    foreach (var img in uploadedImages)
                        productReview.AddImage(img.Url, img.PublicId);

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
                    _logger.LogError(
                        "Failed to delete image with PublicId: {PublicId} during rollback.",
                        publicId
                    );
                }
            }

            throw;
        }
    }

    public async Task<List<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync(
            r => r.ParentId == null
        );

        var orderedReviews = reviews
            .OrderByDescending(r => r.CreateTimeStamp)
            .ToList();

        var userIds = orderedReviews
            .Select(r => r.CreateBy)
            .Distinct()
            .ToList();

        var users = await _userRepository.GetAllAsync(
            u => userIds.Contains(u.Id),
            include: q => q.Include(u => u.Images)
        );

        var userDict = users.ToDictionary(u => u.Id);

        var result = new List<ReviewDto>();

        foreach (var r in orderedReviews)
        {
            CreateReviewDto createReview = new();

            if (r.Metadata != null)
            {
                try
                {
                    if (r.Metadata is JsonElement je)
                    {
                        createReview = je.Deserialize<CreateReviewDto>(
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }
                        ) ?? new CreateReviewDto();
                    }
                    else if (r.Metadata is string s)
                    {
                        createReview = JsonSerializer.Deserialize<CreateReviewDto>(
                            s,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }
                        ) ?? new CreateReviewDto();
                    }
                    else
                    {
                        var json = JsonSerializer.Serialize(r.Metadata);
                        createReview = JsonSerializer.Deserialize<CreateReviewDto>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }
                        ) ?? new CreateReviewDto();
                    }
                }
                catch
                {
                    createReview = new CreateReviewDto();
                }
            }

            // ===============================
            // USER
            // ===============================
            userDict.TryGetValue(r.CreateBy, out var user);

            result.Add(new ReviewDto
            {
                Id = r.Id,
                Comment = r.Comment,
                Rating = r.Rating,
                Metadata = r.Metadata,
                ParentId = r.ParentId,
                Status = r.Status,
                CreateReview = createReview,
                ProductReview = null,

                User = user == null
                    ? null
                    : new UserDto
                    {
                        ID = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        AvatarUrl = user.Images?
                            .OrderByDescending(i => i.Id)
                            .FirstOrDefault()?.Url
                    }
            });
        }

        return result;
    }
    public async Task<ReviewDto?> GetReviewDetailsAsync(int id)
    {
        // ===== Review cha =====
        var parent = await _reviewRepository.GetAsync(r => r.Id == id && r.ParentId == null);
        if (parent == null) return null;

        // ===== Parse CreateReview =====
        CreateReviewDto createReview = new();

        if (parent.Metadata != null)
        {
            try
            {
                var json = parent.Metadata is JsonElement je
                    ? je.GetRawText()
                    : JsonSerializer.Serialize(parent.Metadata);

                createReview = JsonSerializer.Deserialize<CreateReviewDto>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new CreateReviewDto();
            }
            catch { }
        }

        // ===== Review con (product reviews) =====
        var children = await _reviewRepository.GetAllAsync(
            r => r.ParentId == parent.Id && r.Metadata != null
        );

        var productReviews = new List<ProductReviewDto>();

        foreach (var r in children)
        {
            try
            {
                var json = r.Metadata is JsonElement je
                    ? je.GetRawText()
                    : JsonSerializer.Serialize(r.Metadata);

                var meta = JsonSerializer.Deserialize<ProductReviewMetadata>(json);
                if (meta == null) continue;

                productReviews.Add(new ProductReviewDto
                {
                    OrderDetailId = meta.OrderDetailId,
                    ProductId = meta.ProductId,
                    ProductComment = r.Comment,
                    ProductRating = r.Rating,
                    UserId = meta.UserId,
                    ImageUrls = meta.Images?.Select(i => i.Url).ToList() ?? []
                });
            }
            catch { }
        }

        // ===== User =====
        var user = await _userRepository.GetAllAsync(
            u => u.Id == parent.CreateBy,
            include: q => q.Include(u => u.Images)
        );

        var u = user.FirstOrDefault();

        return new ReviewDto
        {
            Id = parent.Id,
            Comment = parent.Comment,
            Rating = parent.Rating,
            Metadata = parent.Metadata,
            ParentId = parent.ParentId,
            Status = parent.Status,
            CreateReview = createReview,
            ProductReview = productReviews,

            User = u == null
                ? null
                : new UserDto
                {
                    ID = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarUrl = u.Images?
                        .OrderByDescending(i => i.Id)
                        .FirstOrDefault()?.Url
                }
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var parent = await _reviewRepository.GetByIdAsync(id);
        if (parent == null)
            return false;

        var children = await _reviewRepository.GetAllAsync(
            r => r.ParentId == id
        );

        if (children.Any())
        {
            await _reviewRepository.DeleteRangeAsync(children);
        }

        await _reviewRepository.DeleteAsync(parent);

        await _reviewRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, ReviewStatus status)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
            return false;

        var now = DateTime.UtcNow;
        var adminId = GetCurrentAdminId();

        // Cập nhật review cha
        review.Status = status;
        review.LastAction = LastAction.Update;
        review.LastActionBy = adminId;
        review.LastActionTimeStamp = now;

        // Lấy tất cả con
        var children = await _reviewRepository.GetAllAsync(r => r.ParentId == id);
        foreach (var child in children)
        {
            child.Status = status;
            child.LastAction = LastAction.Update;
            child.LastActionBy = adminId;
            child.LastActionTimeStamp = now;
        }

        // Chỉ cần gọi UpdateAsync một lần cho cha nếu repository yêu cầu
        await _reviewRepository.UpdateAsync(review);

        // Lưu tất cả thay đổi (cha + con)
        await _reviewRepository.SaveChangesAsync();
        return true;
    }

}