using ShoesShop.Domain.Categories.Dtos;

namespace ShoesShop.Web.Product.Model.Admin;

public class CreateProductAdminDto
{
    public required string Name { get; set; }

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public List<IFormFile> ImageFiles { get; set; } = new();

    public decimal? SaleOff { get; set; }

    public required string Brand { get; set; }

    public string? Sizes { get; set; }

    public string? Color { get; set; }
    
    public List<CategoryDto> Categories { get; set; } = new();
}