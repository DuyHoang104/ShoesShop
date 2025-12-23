using ShoesShop.Domain.Modules.User.Categories.Dtos;
using ShoesShop.Domain.Modules.User.Categories.Entities;
using ShoesShop.Domain.Modules.User.Categories.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;

namespace ShoesShop.Domain.Services.Modules.Users.Categories.Services;
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
            Description = c.Description,
            Status = c.Status,
            LastActionTimeStamp = c.LastActionTimeStamp,
            CreateTimeStamp = c.CreateTimeStamp,
            CreateBy = c.CreateBy,
            LastActionBy = c.LastActionBy,
            LastAction = c.LastAction
        }).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description,
            Status = categoryDto.Status,
            CreateBy = categoryDto.CreateBy,
            CreateTimeStamp = categoryDto.CreateTimeStamp,
            LastActionBy = categoryDto.LastActionBy,
            LastActionTimeStamp = categoryDto.LastActionTimeStamp,
            LastAction = categoryDto.LastAction
        };

        var createdCategory = await _categoryRepository.InsertAsync(category);
        await _categoryRepository.SaveChangesAsync();
        
        return new CategoryDto
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            Status = createdCategory.Status,
            CreateBy = createdCategory.CreateBy,
            CreateTimeStamp = createdCategory.CreateTimeStamp,
            LastActionBy = createdCategory.LastActionBy,
            LastActionTimeStamp = createdCategory.LastActionTimeStamp,
            LastAction = createdCategory.LastAction
        };
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Category with ID {id} not found.");

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            LastActionTimeStamp = category.LastActionTimeStamp,
            CreateTimeStamp = category.CreateTimeStamp,
            CreateBy = category.CreateBy,
            LastActionBy = category.LastActionBy,
            LastAction = category.LastAction
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
            Description = c.Description,
            Status = c.Status,
            LastActionTimeStamp = c.LastActionTimeStamp
        }).ToList();
    }

    public async Task<bool> UpdateAsync(CategoryDto categoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryDto.Id);
        if (category == null)
            return false;

        if (!string.IsNullOrEmpty(categoryDto.Name))
            category.Name = categoryDto.Name;

        if (!string.IsNullOrEmpty(categoryDto.Description))
            category.Description = categoryDto.Description;

        category.Status = categoryDto.Status;
        category.LastActionBy = categoryDto.LastActionBy;
        category.LastActionTimeStamp = DateTime.UtcNow;
        category.LastAction = categoryDto.LastAction;

        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int categoryId)
    {
        await _categoryRepository.DeleteAsync(categoryId);
        await _categoryRepository.SaveChangesAsync();
        return true;
    }
}