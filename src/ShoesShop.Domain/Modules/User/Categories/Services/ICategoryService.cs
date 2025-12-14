using ShoesShop.Domain.Modules.User.Categories.Dtos;

namespace ShoesShop.Domain.Modules.User.Categories.Services;

public interface ICategoryService
{
    public Task<CategoryDto> GetByIdAsync(int id);

    public Task<IEnumerable<CategoryDto>> GetAllAsync();

    public Task<IEnumerable<CategoryDto>> GetListByIdAsync(IEnumerable<int> ids);

    public Task<CategoryDto> CreateAsync(CategoryDto categoryDto);

    public Task<bool> UpdateAsync(CategoryDto categoryDto);

    public Task<bool> DeleteAsync(CategoryDto categoryDto);
}