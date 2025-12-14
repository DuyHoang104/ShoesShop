using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Domain.Modules.User.Carts.Dtos;
public class CartDto
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public ProductDto Product { get; set; }

    public UserDto User { get; set; }

    public string Size { get; set; }

    public decimal Discount { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal TotalPrice => (Product?.Price ?? 0) * Quantity;
}