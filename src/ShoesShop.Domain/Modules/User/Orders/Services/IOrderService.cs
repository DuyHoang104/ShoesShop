using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Orders.Enums;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Domain.Modules.User.Orders.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllOrderAsync(int? userId);
    Task<OrderDetailDto> CreateOrderAsync(OrderCheckoutDto orderDto, int userId);
    Task<decimal> CalculateOrderTotalAsync(int userId, decimal shippingFee = 0, decimal discount = 0);
    Task<OrderDetailDto> GetOrderDetailByIdAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    Task<bool> GetUserInfoForOrderAsync(int userId, int orderId);
}