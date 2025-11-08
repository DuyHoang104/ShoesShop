namespace ShoesShop.Web.Modules.Order.Dtos.Commands
{
    public class OrderDetailItemModalDto
    {
        public string? ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string? ProductImage { get; set; } = null!; 
        public string? ProductSize { get; set; } = null!;
    }
}