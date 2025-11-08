using ShoesShop.Domain.Modules.Orders.Enums;
using ShoesShop.Web.Modules.Order.Dtos.Commands;

namespace ShoesShop.Web.Modules.Order.Dtos
{
    public class OrderModalDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public List<OrderDetailItemModalDto> OrderDetails { get; set; } = [];
        
    }
}