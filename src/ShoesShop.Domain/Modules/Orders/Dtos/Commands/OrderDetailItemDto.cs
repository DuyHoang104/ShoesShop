namespace ShoesShop.Domain.Modules.Orders.Dtos.Commands
{
    public class OrderDetailItemDto
    {
        public string? ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string? ProductImage { get; set; } = null!;
    }
}