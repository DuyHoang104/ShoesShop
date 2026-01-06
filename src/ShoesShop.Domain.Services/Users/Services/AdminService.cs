using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Orders.Entities;
using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Products.Commands.Dtos;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Shares.Image.Dtos;
using ShoesShop.Domain.Shares.Image.Entities;
using ShoesShop.Domain.Shares.Image.Enums;
using ShoesShop.Domain.Users.Dtos;
using ShoesShop.Domain.Users.Entities;
using ShoesShop.Domain.Users.Enums;
using ShoesShop.Domain.Users.Services;

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
        var products = await _productRepository.GetAllAsync();

        var users = await _userRepository.GetAllAsync();
        int userCount = users.Count(u => u.Role != UserAccountRole.Admin);

        var orders = await _orderRepository.GetAllAsync(
            include: q => q
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
        );

        var filteredOrders = orders
            .Where(o => o.User.Role != UserAccountRole.Admin)
            .ToList();

        int orderCount = filteredOrders.Count;

        // Orders by date (chart)
        var ordersByDate = filteredOrders
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new OrderByDateDto
            {
                Date = g.Key,
                Total = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToList();

        // TOTAL REVENUE
        decimal orderRevenue = filteredOrders.Sum(o =>
        {
            var subtotal = o.OrderDetails.Sum(od => od.Subtotal);
            var discount = o.Discount ?? 0m;
            var shippingFee = o.ShippingFee ?? 0m;
            return Math.Max(subtotal - (subtotal * discount) + shippingFee, 0);
        });

        // TODAY REVENUE
        var today = DateTime.Today;

        decimal todayRevenue = filteredOrders
            .Where(o => o.OrderDate.Date == today)
            .Sum(o =>
            {
                var subtotal = o.OrderDetails.Sum(od => od.Subtotal);
                var discount = o.Discount ?? 0m;
                var shippingFee = o.ShippingFee ?? 0m;
                return Math.Max(subtotal - (subtotal * discount) + shippingFee, 0);
            });

        // ORDERS BY LOCATION
        var ordersByLocation = filteredOrders
            .Where(o => !string.IsNullOrWhiteSpace(o.ReceiverAddress))
            .GroupBy(o => o.ReceiverAddress.Trim())
            .Select(g => new OrdersByLocationDto
            {
                Location = g.Key,
                TotalOrders = g.Count()
            })
            .OrderByDescending(x => x.TotalOrders)
            .Take(5)
            .ToList();

        // Products
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
            OrderRevenue = orderRevenue,
            TodayRevenue = todayRevenue,
            OrdersByDate = ordersByDate,
            OrdersByLocation = ordersByLocation
        };
    }
}