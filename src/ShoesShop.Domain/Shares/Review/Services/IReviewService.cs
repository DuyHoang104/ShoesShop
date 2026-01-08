using ShoesShop.Domain.Shares.Review.Dtos;
using ShoesShop.Domain.Shares.Review.Enums;

namespace ShoesShop.Domain.Shares.Review.Services;

public interface IReviewService
{
    public Task CreateReviewAsync(int userId, ReviewDto dto);

    public Task<List<ReviewDto>> GetAllReviewsAsync();

    public Task<ReviewDto?> GetReviewDetailsAsync(int id);

    Task<bool> DeleteAsync(int reviewId);

    Task<bool> UpdateStatusAsync(int id, ReviewStatus status);
}