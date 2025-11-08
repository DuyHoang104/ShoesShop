using ShoesShop.Domain.Modules.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.Orders.Enums;

namespace ShoesShop.Domain.Modules.Orders.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<OrderDetailItemDto> OrderDetails { get; set; } = [];
        public decimal SubTotal { get; set; }
    }
}