using ShoesShop.Domain.Modules.User.Products.Enums;

namespace ShoesShop.Domain.Modules.User.Products.Commands.Dtos;

public class ProductUpdateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal? SaleOff { get; set; }
    public ProductStatus Status { get; set; }
    public List<int> Categories { get; set; } = new();
}
