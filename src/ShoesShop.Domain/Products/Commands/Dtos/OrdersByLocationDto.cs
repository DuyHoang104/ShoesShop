namespace ShoesShop.Domain.Products.Commands.Dtos;

public class OrdersByLocationDto
{
    public string Location { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
}