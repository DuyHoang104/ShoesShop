using ShoesShop.Domain.Products.Enums;

namespace ShoesShop.Domain.Products.Commands.Dtos;

public class ProductUpdateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal? SaleOff { get; set; }
    public ProductStatus Status { get; set; }
    public List<int> Categories { get; set; } = new();
}
