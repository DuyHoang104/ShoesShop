using ShoesShop.Domain.Products.Commands;

namespace ShoesShop.Domain.Users.Dtos;

public class AdminDto
{
    public List<ProductDto> Products { get; set; } = [];
    public int UserCount { get; set; }
    public int OrderCount { get; set; }
    public decimal OrderRevenue { get; set; }
}