using ShoesShop.Domain.Carts.Dtos;
using ShoesShop.Domain.Categories.Dtos;
using ShoesShop.Domain.Commons.Enums;
using ShoesShop.Domain.Orders.Dtos.Commands;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Shares.Image.Dtos;

namespace ShoesShop.Web.Product.Model.Admin;

public class ProductAdminDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal? SaleOff { get; set; }

    public ProductStatus? Status { get; set; }
    
    public ProductStockStatus? StockStatus { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Sizes { get; set; } = string.Empty;

    public List<CategoryDto> Categories { get; set; } = [];

    public List<OrderDetailDto> OrderDetails { get; set; } = [];

    public List<CartResponse> Carts { get; set; } = [];

    public List<ImageDto> Images { get; set; } = [];

    public LastAction LastAction { get; set; }

    public DateTime CreateTimeStamp { get; set; }
}