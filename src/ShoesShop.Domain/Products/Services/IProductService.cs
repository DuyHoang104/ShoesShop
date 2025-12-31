using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Products.Commands.Dtos;

namespace ShoesShop.Domain.Products.Services;

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