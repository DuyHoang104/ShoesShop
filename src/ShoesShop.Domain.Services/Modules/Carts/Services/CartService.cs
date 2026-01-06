using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Carts.Dtos;
using ShoesShop.Domain.Carts.Entities;
using ShoesShop.Domain.Carts.Services;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Users.Entities;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Shares.Image.Dtos;
using ShoesShop.Domain.Users.Entities;

namespace ShoesShop.Domain.Services.Modules.Carts.Services;

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

            if (product.StockStatus == ProductStockStatus.OutOfStock)
                throw new BusinessException("Product is not available in stock");

            var newItem = new Cart(user, product, quantity, size);
            await _cartRepository.InsertAsync(newItem);
        }

        await _cartRepository.SaveChangesAsync();
    }

    public async Task<List<CartResponse>> GetByUserIdAsync(int userId)
    {
        var items = (await _cartRepository.GetAllAsync(
        include: q => q
            .Include(c => c.Product)
            .ThenInclude(p => p.Images)))
        .Where(c => c.UserId == userId)
        .ToList();

        return items.Select(c => new CartResponse
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
                Images = c.Product.Images?.Select(i => new ImageDto { Url = i.Url }).ToList() ?? [],
                Quantity = c.Product.Quantity,
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

    public async Task UpdateCartAsync(List<CartResponse> items, int userId)
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