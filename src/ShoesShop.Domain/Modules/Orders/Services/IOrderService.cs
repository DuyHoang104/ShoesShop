using ShoesShop.Domain.Modules.Orders.Dtos;

namespace ShoesShop.Domain.Modules.Orders.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(OrderDto orderDto);
        Task<decimal> CalculateOrderTotalAsync(decimal shippingFee = 0, decimal discount = 0);

    }
}