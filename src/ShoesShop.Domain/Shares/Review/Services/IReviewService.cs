using ShoesShop.Domain.Shares.Review.Dtos;

namespace ShoesShop.Domain.Shares.Review.Services;

public interface IReviewService
{
    public Task CreateReviewAsync(int userId, ReviewDto dto);
}