using ShoesShop.Domain.Modules.Categories.Dtos;

namespace ShoesShop.Domain.Modules.Categories.Services
{
    public interface ICategoryService
    {
        public Task<CategoryDto> GetByIdAsync(int id);

        public Task<IEnumerable<CategoryDto>> GetAllAsync();

        public Task<IEnumerable<CategoryDto>> GetListByIdAsync(IEnumerable<int> ids);
    }
}