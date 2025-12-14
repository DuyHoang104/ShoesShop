using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.User.Carts.Dtos;
using ShoesShop.Domain.Modules.User.Carts.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;
using ShoesShop.Domain.Modules.User.Products.Entities;
using ShoesShop.Domain.Modules.User.Shares.Dtos;
using ShoesShop.Domain.Modules.User.Users.Entities;

namespace ShoesShop.Domain.Services.Modules.Users.Carts.Services;
public class CartService : ICartService
{
    private readonly IGenericRepository<Cart, int> _cartRepository;
    private readonly IGenericRepository<User, int> _userRepository;
    private readonly IGenericRepository<Product, int> _productRepository;
    private const string CartSessionKey = "CartSession";

    public CartService(
        IGenericRepository<Cart, int> cartRepository,
        IGenericRepository<User, int> userRepository,
        IGenericRepository<Product, int> productRepository)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public async Task AddToCartAsync(int productId, int quantity, string size, int userId)
    {
        var existing = (await _cartRepository.GetAllAsync())
            .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId && c.Size == size);

        if (existing != null)
        {
            existing.Quantity += quantity;
            await _cartRepository.UpdateAsync(existing);
        }
        else
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new BusinessException("User not found");

            var product = (await _productRepository.GetAllAsync(
                include: q => q.Include(p => p.Images)))
                .FirstOrDefault(p => p.Id == productId)
                ?? throw new BusinessException("Product not found");

            var newItem = new Cart(user, product, quantity, size);
            await _cartRepository.InsertAsync(newItem);
        }

        await _cartRepository.SaveChangesAsync();
    }
    
    public async Task<List<CartDto>> GetByUserIdAsync(int userId)
    {
        var items = (await _cartRepository.GetAllAsync(
        include: q => q
            .Include(c => c.Product)
            .ThenInclude(p => p.Images)))
        .Where(c => c.UserId == userId)
        .ToList();

        return items.Select(c => new CartDto
        {
            Id = c.Id,
            ProductId = c.ProductId,
            Quantity = c.Quantity,
            Size = c.Size,
            Product = new ProductDto
            {
                Id = c.Product.Id,
                Name = c.Product.Name,
                Price = c.Product.Price,
                Images = c.Product.Images?.Select(i => new ImageDto { Url = i.Url }).ToList() ?? []
            }
        }).ToList();
    }

    public async Task RemoveFromCartAsync(int productId, string size, int userId)
    {
        var existing = (await _cartRepository.GetAllAsync())
            .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId && c.Size == size);

        if (existing != null)
        {
            await _cartRepository.DeleteAsync(existing);
            await _cartRepository.SaveChangesAsync();
        }
    }

    public async Task UpdateCartAsync(List<CartDto> items, int userId)
    {
        var carts = await _cartRepository.GetAllAsync(c => c.UserId == userId);

        foreach (var item in items)
        {
            var existing = carts.FirstOrDefault(c => c.ProductId == item.ProductId && c.Size == item.Size);
            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                await _cartRepository.UpdateAsync(existing);
            }
        }

        await _cartRepository.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int userId)
    {
        var carts = await _cartRepository.GetAllAsync(c => c.UserId == userId);
        if (carts.Any())
        {
            await _cartRepository.DeleteRangeAsync(carts);
            await _cartRepository.SaveChangesAsync();
        }
    }
}
