using ShoesShop.Domain.Modules.Categories.Dtos;
using ShoesShop.Domain.Modules.Products.Enums;

namespace ShoesShop.Web.Modules.Product.Dtos
{
    public class CreateModalDto
    {
        public required string Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public List<IFormFile> ImageFiles { get; set; } = new();

        public decimal? SaleOff { get; set; }

        public required string Brand { get; set; }

        public string Sizes { get; set; } = string.Empty;

        public string? Color { get; set; }

        public ProductStatus? Status { get; set; }
        
        public List<CategoryDto> Categories { get; set; } = new();
    }
}