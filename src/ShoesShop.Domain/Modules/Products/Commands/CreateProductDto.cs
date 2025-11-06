using ShoesShop.Domain.Modules.Carts.Dtos;
using ShoesShop.Domain.Modules.Categories.Dtos;
using ShoesShop.Domain.Modules.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.Products.Enums;

namespace ShoesShop.Domain.Modules.Products.Dtos.Commands
{
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

        public List<CartItemDto> CartItems { get; set; }
        
        public List<string>? ImageUrl { get; set; }
    }
}