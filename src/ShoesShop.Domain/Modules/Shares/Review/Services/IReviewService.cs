using ShoesShop.Domain.Modules.Shares.Review.Dtos;

namespace ShoesShop.Domain.Modules.Shares.Review.Services;

public interface IReviewService
{
    public Task CreateReviewAsync(int userId, ReviewDto dto);
}