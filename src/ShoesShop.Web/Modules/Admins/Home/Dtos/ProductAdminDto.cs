using ShoesShop.Domain.Modules.User.Carts.Dtos;
using ShoesShop.Domain.Modules.User.Categories.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Products.Enums;
using ShoesShop.Domain.Modules.User.Shares.Dtos;

namespace ShoesShop.Web.Modules.Admins.Home.Dtos;
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

    public int UserCount { get; set; }

    public int OrderCount { get; set; }

    public decimal OrderRevenue { get; set; }
}