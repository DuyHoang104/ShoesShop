using ShoesShop.Domain.Modules.User.Orders.Enums;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Web.Modules.Admins.Orders.Dtos;
public class OrderDetailAdminModalDto
{
    public int Id { get; set; }
    public string? ReceiverName { get; set; } = null!;
    public string? ReceiverPhone { get; set; } = null!;
    public string? ReceiverAddress { get; set; } = null!;
    public string? ReceiverCity { get; set; } = null!;
    public string? ReceiverCountry { get; set; } = null!;
    public AddressModalAdminDto? Address { get; set; } = null!;
    public string? Note { get; set; } = string.Empty;
    public decimal? ShippingCost { get; set; }
    public decimal? DiscountValue { get; set; }
    public PaymentMethod? PaymentMethod { get; set; } = null!;
    public PaymentStatus? PaymentStatus { get; set; } = null!;
    public OrderStatus? Status { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderDetailItemAdminModalDto> OrderDetails { get; set; } = [];
    public UserDto User { get; set; } = null!;
}