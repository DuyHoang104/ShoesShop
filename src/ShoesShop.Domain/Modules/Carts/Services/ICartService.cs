using ShoesShop.Domain.Modules.Carts.Dtos;

namespace ShoesShop.Domain.Modules.Carts.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity, string size);

        Task RemoveFromCartAsync(int productId, string size);

        Task<List<CartItemDto>> GetByUserIdAsync();

        Task UpdateCartAsync(List<CartItemDto> items);

        Task ClearCartAsync();
        // Task AddShippingAsync(string country, string city, string address);
    }
}