using ShoesShop.Domain.Orders.Enums;
using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Domain.Orders.Dtos.Commands;

public class OrderDetailDto
{
    public int Id { get; set; }
    public string? ReceiverName { get; set; } = null!;
    public string? ReceiverPhone { get; set; } = null!;
    public string? ReceiverAddress { get; set; } = null!;
    public string? ReceiverCity { get; set; } = null!;
    public string? ReceiverCountry { get; set; } = null!;
    public string? Note { get; set; } = string.Empty;
    public decimal? ShippingCost { get; set; }
    public decimal? DiscountValue { get; set; }
    public PaymentMethod? PaymentMethod { get; set; } = null!;
    public PaymentStatus? PaymentStatus { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public AddressDto? Address { get; set; }
    public List<OrderDetailItemDto> OrderDetails { get; set; } = [];
    public UserDto User { get; set; } = null!;
}