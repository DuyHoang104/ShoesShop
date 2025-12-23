namespace ShoesShop.Web.Modules.Admins.Orders.Dtos;

public class OrderDetailItemAdminModalDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public string? ProductImage { get; set; } = null!; 
    public string? Size { get; set; } = null!;
}