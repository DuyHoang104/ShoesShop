using ShoesShop.Domain.Modules.User.Orders.Enums;

namespace ShoesShop.Web.Modules.Admins.Orders.Dtos;

public class OrderAdminDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal? ShippingFee { get; set; }
    public decimal? Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public List<OrderDetailItemAdminModalDto> OrderDetails { get; set; } = [];
    public decimal SubTotal { get; set; }
}
