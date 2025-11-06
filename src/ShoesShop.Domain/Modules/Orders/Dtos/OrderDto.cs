using ShoesShop.Domain.Modules.Orders.Enums;

namespace ShoesShop.Domain.Modules.Orders.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public bool SameAddress { get; set; }
        public string? ReceiverName { get; set; } = null!;
        public string? ReceiverPhone { get; set; } = null!;
        public string? City { get; set; } = null!;
        public string? Country { get; set; } = null!;
        public string? AddressLine1 { get; set; } = null!;
        public int AddressId { get; set; }
        public string? Note { get; set; } = string.Empty;
        public string? ReceiverAddress { get; set; } = null!;
        public decimal? ShippingFee { get; set; }
        public decimal? Discount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}