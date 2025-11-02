using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.Carts.Dtos;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Products.Entities;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Domain.Modules.Shares.Dtos;
using ShoesShop.Crosscutting.Utilities.Exceptions;

namespace ShoesShop.Domain.Services.Modules.Carts.Services
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<CartItem, int> _cartItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericRepository<User, int> _userRepository;
        private readonly IGenericRepository<Product, int> _productRepository;
        private const string CartSessionKey = "CartSession";

        public CartService(
            IGenericRepository<CartItem, int> cartItemRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<User, int> userRepository,
            IGenericRepository<Product, int> productRepository)
        {
            _cartItemRepository = cartItemRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        private int GetUserIdFromClaims()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
             if (string.IsNullOrEmpty(userIdClaim))
                return 0;

            return int.Parse(userIdClaim);
        }
        
        public async Task AddToCartAsync(int productId, int quantity, string size)
        {
            var userId = GetUserIdFromClaims();
            if (userId == 0)
                return;

            var existing = (await _cartItemRepository.GetAllAsync())
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId && c.Size == size);

            if (existing != null)
            {
                existing.Quantity += quantity;
                await _cartItemRepository.UpdateAsync(existing);
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new BusinessException("User not found");

                var product = (await _productRepository.GetAllAsync(
                    include: q => q.Include(p => p.Images)))
                    .FirstOrDefault(p => p.Id == productId)
                    ?? throw new BusinessException("Product not found");

                var newItem = new CartItem(user, product, quantity, size);
                await _cartItemRepository.InsertAsync(newItem);
            }

            await _cartItemRepository.SaveChangesAsync();
        }
        
        public async Task<List<CartItemDto>> GetByUserIdAsync()
        {
            var userId = GetUserIdFromClaims();

            var items = (await _cartItemRepository.GetAllAsync(
            include: q => q
                .Include(c => c.Product)
                .ThenInclude(p => p.Images)))
            .Where(c => c.UserId == userId)
            .ToList();

            return items.Select(c => new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                Size = c.Size,
                Product = new Domain.Modules.Products.Dtos.ProductDto
                {
                    Id = c.Product.Id,
                    Name = c.Product.Name,
                    Price = c.Product.Price,
                    Images = c.Product.Images?.Select(i => new ImageUploadDto { Url = i.Url }).ToList() ?? []
                }
            }).ToList();
        }

        public async Task RemoveFromCartAsync(int productId, string size)
        {
            var userId = GetUserIdFromClaims();

            var existing = (await _cartItemRepository.GetAllAsync())
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId && c.Size == size);

            if (existing != null)
            {
                await _cartItemRepository.DeleteAsync(existing);
                await _cartItemRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateCartAsync(List<CartItemDto> items)
        {
            var userId = GetUserIdFromClaims();

            var cartItems = await _cartItemRepository.GetAllAsync(c => c.UserId == userId);

            foreach (var item in items)
            {
                var existing = cartItems.FirstOrDefault(c => c.ProductId == item.ProductId && c.Size == item.Size);
                if (existing != null)
                {
                    existing.Quantity = item.Quantity;
                    await _cartItemRepository.UpdateAsync(existing);
                }
            }

            await _cartItemRepository.SaveChangesAsync();
        }

        public async Task ClearCartAsync()
        {
            var userId = GetUserIdFromClaims();

            var cartItems = await _cartItemRepository.GetAllAsync(c => c.UserId == userId);
            if (cartItems.Any())
            {
                await _cartItemRepository.DeleteRangeAsync(cartItems);
                await _cartItemRepository.SaveChangesAsync();
            }
        }

        // public async Task AddShippingAsync(string country, string city, string address)
        // {
        //     var userId = GetUserIdFromClaims();

        //     var user = (await _userRepository.GetAllAsync(
        //         u => u.Id == userId,
        //         include: q => q.Include(u => u.Addresses)
        //     )).FirstOrDefault()
        //         ?? throw new BusinessException("User not found");

        //     var existingAddress = user.Addresses.FirstOrDefault(a =>
        //         a.Country == country && a.City == city && a.AddressLine1 == address);

        //     if (existingAddress != null)
        //         throw new BusinessException("This address already exists.");

        //     var newAddress = new Address
        //     {
        //         Country = country,
        //         City = city,
        //         AddressLine1 = address,
        //         IsDefault = !user.Addresses.Any()
        //     };

        //     user.AddAddress(newAddress);
        //     await _userRepository.UpdateAsync(user);
        //     await _userRepository.SaveChangesAsync();
        // }


    }
}