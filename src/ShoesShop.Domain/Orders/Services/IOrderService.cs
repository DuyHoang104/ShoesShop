using ShoesShop.Domain.Orders.Dtos;
using ShoesShop.Domain.Orders.Dtos.Commands;
using ShoesShop.Domain.Orders.Enums;

namespace ShoesShop.Domain.Orders.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllOrderAsync(int? userId);
    Task<OrderDetailDto> CreateOrderAsync(OrderCheckoutDto orderDto, int userId, CancellationToken cancellationToken = default);
    Task<decimal> CalculateOrderTotalAsync(int userId, decimal shippingFee = 0, decimal discount = 0);
    Task<OrderDetailDto> GetOrderDetailByIdAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    Task<bool> CheckUserInfoForOrderAsync(int userId, int orderId);
}