using ShoesShop.Domain.Modules.User.Carts.Dtos;
using ShoesShop.Domain.Modules.User.Categories.Dtos;
using ShoesShop.Domain.Modules.User.Commons.Enums;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Products.Enums;
using ShoesShop.Domain.Modules.User.Shares.Dtos;

namespace ShoesShop.Web.Modules.Admins.Products.Dtos;
public class ProductAdminDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal? SaleOff { get; set; }

    public ProductStatus? Status { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Sizes { get; set; } = string.Empty;

    public List<CategoryDto> Categories { get; set; } = [];

    public List<OrderDetailDto> OrderDetails { get; set; } = [];

    public List<CartDto> Carts { get; set; } = [];

    public List<ImageDto> Images { get; set; } = [];

    public LastAction LastAction { get; set; }

    public DateTime CreateTimeStamp { get; set; }
}