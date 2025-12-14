using ShoesShop.Domain.Modules.User.Carts.Dtos;
using ShoesShop.Domain.Modules.User.Categories.Dtos;
using ShoesShop.Domain.Modules.User.Commons.Enums;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Products.Enums;

namespace ShoesShop.Domain.Modules.User.Products.Commands;

public class CreateProductDto
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public int Quantity { get; set; }
    
    public string Brand { get; set; }

    public string Color { get; set; }

    public string Sizes { get; set; } = string.Empty;

    public decimal? SaleOff { get; set; }

    public ProductStatus? Status { get; set; }

    public List<CategoryDto> Categories { get; set; }

    public List<OrderDetailDto> OrderDetails { get; set; }

    public List<CartDto> Carts { get; set; }
    
    public List<string>? ImageUrl { get; set; }

    public int CreateBy { get; set; }

    public DateTime CreateTimeStamp { get; set; }

    public int LastActionBy { get; set; }

    public LastAction LastAction { get; set; }

    public DateTime LastActionTimeStamp { get; set; }
}