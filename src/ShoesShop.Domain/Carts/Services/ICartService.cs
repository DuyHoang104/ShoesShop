using ShoesShop.Domain.Carts.Dtos;

namespace ShoesShop.Domain.Carts.Services;

public interface ICartService
{
    Task AddToCartAsync(int productId, int quantity, string size, int userId);
    Task RemoveFromCartAsync(int productId, string size, int userId);
    Task UpdateCartAsync(List<CartResponse> items, int userId);
    Task ClearCartAsync(int userId);
    Task<List<CartResponse>> GetByUserIdAsync(int userId);
}