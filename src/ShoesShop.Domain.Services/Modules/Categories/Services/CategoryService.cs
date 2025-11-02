using ShoesShop.Domain.Modules.Categories.Dtos;
using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Categories.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;

namespace ShoesShop.Domain.Services.Modules.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category, int> _categoryRepository;

        public CategoryService(IGenericRepository<Category, int> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<IEnumerable<CategoryDto>> GetListByIdAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return [];

            var categories = await _categoryRepository.GetAllAsync(c => ids.Contains(c.Id));

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();
        }
    }
}