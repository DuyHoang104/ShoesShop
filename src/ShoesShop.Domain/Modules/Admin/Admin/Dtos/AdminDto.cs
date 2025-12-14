using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Domain.Modules.Admin.Admin.Dtos
{
    public class AdminDto
    {
        public List<ProductDto> Products { get; set; } = [];
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public decimal OrderRevenue { get; set; }
    }
}