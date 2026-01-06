using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Products.Commands.Dtos;

namespace ShoesShop.Domain.Users.Dtos;

public class AdminDto
{
    public List<ProductDto> Products { get; set; } = [];
    public int UserCount { get; set; }
    public int OrderCount { get; set; }
    public decimal OrderRevenue { get; set; }
    public List<OrderByDateDto> OrdersByDate { get; set; } = new();
    public decimal TodayRevenue { get; set; }
    public List<OrdersByLocationDto> OrdersByLocation { get; set; } = new();
}