using ShoesShop.Domain.Modules.Orders.Enums;
using ShoesShop.Domain.Modules.Products.Dtos;

namespace ShoesShop.Web.Modules.Order.Dtos.Commands
{
    public class OrderDetailModalDto
    {
        public int Id { get; set; }
        public string? ReceiverName { get; set; } = null!;
        public string? ReceiverPhone { get; set; } = null!;
        public string? ReceiverAddress { get; set; } = null!;
        public string? ReceiverCity { get; set; } = null!;
        public string? ReceiverCountry { get; set; } = null!;
        public AddressModalDto? Address { get; set; } = null!;
        public string? Note { get; set; } = string.Empty;
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountValue { get; set; }
        public PaymentMethod? PaymentMethod { get; set; } = null!;
        public PaymentStatus? PaymentStatus { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailItemModalDto> OrderDetails { get; set; } = [];
    }
}