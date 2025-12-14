using ShoesShop.Domain.Modules.User.Carts.Dtos;

namespace ShoesShop.Domain.Modules.User.Carts.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity, string size, int userId);
        Task RemoveFromCartAsync(int productId, string size, int userId);
        Task UpdateCartAsync(List<CartDto> items, int userId);
        Task ClearCartAsync(int userId);
        Task<List<CartDto>> GetByUserIdAsync(int userId);
    }
}