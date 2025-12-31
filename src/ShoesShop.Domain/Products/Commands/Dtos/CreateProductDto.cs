using Microsoft.AspNetCore.Http;
using ShoesShop.Domain.Carts.Dtos;
using ShoesShop.Domain.Categories.Dtos;
using ShoesShop.Domain.Commons.Enums;
using ShoesShop.Domain.Orders.Dtos.Commands;

namespace ShoesShop.Domain.Products.Commands.Dtos;

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

    public List<CategoryDto> Categories { get; set; }

    public List<OrderDetailDto> OrderDetails { get; set; }

    public List<CartResponse> Carts { get; set; }
    
    public List<IFormFile> ImageFiles { get; set; } = new();
    
    public int CreateBy { get; set; }

    public DateTime CreateTimeStamp { get; set; }

    public int LastActionBy { get; set; }

    public LastAction LastAction { get; set; }

    public DateTime LastActionTimeStamp { get; set; }
}