using ShoesShop.Domain.Modules.User.Products.Commands;
using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;

namespace ShoesShop.Domain.Modules.User.Products.Services;
public interface IProductService
{
    // Admin
    public Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    public Task<bool> UpdateAsync(int id, ProductUpdateDto productDto, int adminId);

    // User
    public Task<IEnumerable<ProductDto>> GetAllAsync();
    public Task<IEnumerable<ProductDto>> GetAllCategoriesAsync();
    public Task<IEnumerable<ProductDto>> SearchAsync(string? query);
    public Task<ProductDto?> GetByIdAsync(int id);
    public Task<bool> DeleteAsync(int id);

}