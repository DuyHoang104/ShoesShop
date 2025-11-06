using ShoesShop.Domain.Modules.Carts.Dtos;
using ShoesShop.Domain.Modules.Categories.Dtos;
using ShoesShop.Domain.Modules.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.Products.Enums;
using ShoesShop.Domain.Modules.Shares.Dtos;

namespace ShoesShop.Domain.Modules.Products.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal? SaleOff { get; set; }

        public ProductStatus? Status { get; set; }

        public string Brand { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Sizes { get; set; } = string.Empty;

        public List<CategoryDto> Categories { get; set; } = [];

        public List<OrderDetailDto> OrderDetails { get; set; } = [];

        public List<CartItemDto> CartItems { get; set; } = [];

        public List<ImageUploadDto> Images { get; set; } = [];
    }
}