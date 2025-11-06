using ShoesShop.Domain.Modules.Orders.Dtos;
using ShoesShop.Domain.Modules.Orders.Dtos.Commands;

namespace ShoesShop.Domain.Modules.Orders.Services
{
    public interface IOrderService
    {
        Task<OrderDetailDto> CreateOrderAsync(OrderDto orderDto);
        Task<decimal> CalculateOrderTotalAsync(decimal shippingFee = 0, decimal discount = 0);
        Task<OrderDetailDto> GetOrderDetailByIdAsync(int orderId);
    }
}