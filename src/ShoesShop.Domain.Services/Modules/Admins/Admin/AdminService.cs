using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.Admin.Admin.Dtos;
using ShoesShop.Domain.Modules.Admin.Admin.Services;
using ShoesShop.Domain.Modules.Shares.Image.Entities;
using ShoesShop.Domain.Modules.Shares.Image.Enums;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;
using ShoesShop.Domain.Modules.User.Products.Entities;
using ShoesShop.Domain.Modules.User.Shares.Dtos;
using ShoesShop.Domain.Modules.User.Users.Entities;
using ShoesShop.Domain.Modules.User.Users.Enums;

namespace ShoesShop.Domain.Services.Modules.Admins.Admin;
public class AdminService : IAdminService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGenericRepository<Product, int> _productRepository;
    private readonly IGenericRepository<User, int> _userRepository;
    private readonly IGenericRepository<Order, int> _orderRepository;
    private readonly IGenericRepository<ImageProduct, int> _imageProductRepository;
    
    public AdminService(
        IHttpContextAccessor httpContextAccessor,
        IGenericRepository<Product, int> productRepository,
        IGenericRepository<User, int> userRepository,
        IGenericRepository<Order, int> orderRepository,
            IGenericRepository<ImageProduct, int> imageProductRepository
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _imageProductRepository = imageProductRepository;
    }

    public async Task<AdminDto> GetAllInfomationAsync()
    {
        // Products (KHÔNG include Images)
        var products = await _productRepository.GetAllAsync();

        // Users
        var users = await _userRepository.GetAllAsync();
        int userCount = users.Count(u => u.Role != UserAccountRole.Admin);

        // Orders (include thật)
        var orders = await _orderRepository.GetAllAsync(
            include: q => q.Include(o => o.OrderDetails)
                        .Include(o => o.User)
        );

        var filteredOrders = orders.Where(o => o.User.Role != UserAccountRole.Admin);
        int orderCount = filteredOrders.Count();

        decimal orderRevenue = filteredOrders.Sum(o =>
        {
            var subtotal = o.OrderDetails.Sum(od => od.Subtotal);
            var discount = o.Discount ?? 0m;
            var shippingFee = o.ShippingFee ?? 0m;
            return Math.Max(subtotal - (subtotal * discount) + shippingFee, 0);
        });

        // Images for products (POLYMORPHIC)
        var productIds = products.Select(p => p.Id).ToList();

        var images = await _imageProductRepository.GetAllAsync(
            i => i.OwnerType == OwnerType.Product
            && productIds.Contains(i.OwnerId)
        );

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            Quantity = p.Quantity,
            SaleOff = p.SaleOff,
            Status = p.Status,
            Brand = p.Brand,
            Color = p.Color,
            Sizes = p.Sizes,
            Images = images
                .Where(i => i.OwnerId == p.Id)
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                })
                .ToList()
        }).ToList();

        return new AdminDto
        {
            Products = productDtos,
            UserCount = userCount,
            OrderCount = orderCount,
            OrderRevenue = orderRevenue
        };
    }

}