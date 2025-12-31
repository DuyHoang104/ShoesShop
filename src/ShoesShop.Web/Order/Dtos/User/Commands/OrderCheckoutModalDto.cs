

using ShoesShop.Domain.Carts.Dtos;

namespace ShoesShop.Web.Modules.Users.Order.Dtos.Commands;
public class OrderCheckoutModalDto
{
    public bool SameAddress { get; set; }
    public string? ReceiverName { get; set; } = null!;
    public string? ReceiverPhone { get; set; } = null!;
    public string? ReceiverAddress { get; set; } = null!;
    public string? ReceiverCity { get; set; } = null!;
    public string? ReceiverCountry { get; set; } = null!;
    public int AddressId { get; set; }
    public string? Note { get; set; } = string.Empty;
    public List<CartResponse> Carts { get; set; } = new();
    public decimal ShippingCost { get; set; }
    public decimal DiscountValue { get; set; }
}