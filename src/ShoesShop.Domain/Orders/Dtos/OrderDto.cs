using ShoesShop.Domain.Orders.Dtos.Commands;
using ShoesShop.Domain.Orders.Enums;
using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Domain.Orders.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public string ReceiverName { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal? ShippingFee { get; set; }
    public decimal? Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public List<OrderDetailItemDto> OrderDetails { get; set; } = [];
    public decimal SubTotal { get; set; }
    public UserDto User { get; set; } = new UserDto();
}