using ShoesShop.Domain.Modules.Products.Dtos;
using ShoesShop.Domain.Modules.Products.Dtos.Commands;

namespace ShoesShop.Domain.Modules.Products.Services
{
    public interface IProductService
    {
        public Task<ProductDto> CreateAsync(CreateProductDto createProductDto);

        public Task<IEnumerable<ProductDto>> GetAllAsync();

        public Task<IEnumerable<ProductDto>> GetAllCategoriesAsync();

        public Task<IEnumerable<ProductDto>> SearchAsync(string? query);

        public Task<ProductDto?> GetByIdAsync(int id);
    }
}