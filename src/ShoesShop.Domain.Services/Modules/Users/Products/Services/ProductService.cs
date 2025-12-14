using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Modules.User.Categories.Dtos;
using ShoesShop.Domain.Modules.User.Categories.Entities;
using ShoesShop.Domain.Modules.User.Categories.Enums;
using ShoesShop.Domain.Modules.User.Categories.Services;
using ShoesShop.Domain.Modules.User.Commons.Enums;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Products.Commands;
using ShoesShop.Domain.Modules.User.Products.Commands.Dtos;
using ShoesShop.Domain.Modules.User.Products.Entities;
using ShoesShop.Domain.Modules.User.Products.Services;
using ShoesShop.Domain.Modules.User.Shares.Dtos;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Users.Products.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Product, int> _productRepository;
    private readonly IGenericRepository<Category, int> _categoryRepository;
    private readonly ICategoryService _categoryService;

    public ProductService(IGenericRepository<Product, int> productRepository, IGenericRepository<Category, int> categoryRepository, ICategoryService categoryService, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _categoryService = categoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.Images));

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            Images = p.Images.Select(i => new ImageDto
            {
                Id = i.Id,
                Url = i.Url
            }).ToList() ?? []
        });
    }

    public async Task<IEnumerable<ProductDto>> GetAllCategoriesAsync()
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images));

        var result = new List<ProductDto>();

        foreach (var p in products)
        {
            var categories = p.ProductCategories?
                .Select(pc => pc.Category)
                .Where(c => c != null)
                .ToList() ?? [];

            var images = p.Images?
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                })
                .ToList() ?? [];

            result.Add(new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                SaleOff = p.SaleOff,
                Brand = p.Brand,
                Color = p.Color,
                Status = p.Status,
                Sizes = p.Sizes,
                LastAction = p.LastAction,
                CreateTimeStamp = p.CreateTimeStamp,
                Categories = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList(),

                Images = images
            });
        }

        return result;
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images));

        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Quantity = product.Quantity,
            SaleOff = product.SaleOff,
            Brand = product.Brand,
            Color = product.Color,
            Status = product.Status,
            Sizes = product.Sizes,

            Categories = product.ProductCategories?
            .Select(pc => new CategoryDto
            {
                Id = pc.Category.Id,
                Name = pc.Category.Name
            })
            .ToList() ?? [],

            Images = product.Images.Select(i => new ImageDto
            {
                Id = i.Id,
                Url = i.Url
            }).ToList() ?? []
        };
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string? query)
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images)
        );

        if (string.IsNullOrWhiteSpace(query))
        {
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Quantity = p.Quantity,
                SaleOff = p.SaleOff,
                Status = p.Status,
                Sizes = p.Sizes,
                Brand = p.Brand,
                Color = p.Color,
                Categories = p.ProductCategories?
                .Select(pc => new CategoryDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToList() ?? [],
            
                Images = p.Images.Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList() ?? []
            });
        }

        var filtered = products
            .Where(p =>
                (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Color) && p.Color.Contains(query, StringComparison.OrdinalIgnoreCase))
            )
            .ToList();

        var result = filtered.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            Quantity = p.Quantity,
            SaleOff = p.SaleOff,
            Status = p.Status,
            Brand = p.Brand,
            Color = p.Color,
            Sizes = p.Sizes,
            Categories = p.ProductCategories?
                            .Select(pc => new CategoryDto
                            {
                                Id = pc.Category.Id,
                                Name = pc.Category.Name
                            }).ToList() ?? [],
            Images = p.Images.Select(i => new ImageDto
            {
                Id = i.Id,
                Url = i.Url
            }).ToList() ?? []
        });

        return result;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        var adminId = createProductDto.CreateBy;

        // --- Tạo Product entity ---
        var product = new Product
        {
            Name = createProductDto.Name,
            Price = createProductDto.Price,
            Description = createProductDto.Description ?? string.Empty,
            Quantity = createProductDto.Quantity,
            SaleOff = createProductDto.SaleOff,
            Status = createProductDto.Status,
            Brand = createProductDto.Brand,
            Color = createProductDto.Color ?? string.Empty,
            Sizes = createProductDto.Sizes,
            CreateBy = adminId,
            CreateTimeStamp = DateTime.UtcNow,
            LastActionBy = adminId,
            LastAction = LastAction.Create,
            LastActionTimeStamp = DateTime.UtcNow
        };

        // --- Xử lý ảnh ---
        if (createProductDto.ImageUrl?.Any() == true)
        {
            foreach (var url in createProductDto.ImageUrl.Where(u => !string.IsNullOrWhiteSpace(u)))
                product.AddImage(url.Trim());
        }

        // --- Xử lý category ---
        if (createProductDto.Categories?.Any() == true)
        {
            foreach (var catDto in createProductDto.Categories)
            {
                Category category;
                if (catDto.Id > 0)
                {
                    category = await _categoryRepository.GetByIdAsync(catDto.Id)
                        ?? throw new InvalidOperationException($"Category with ID {catDto.Id} not found.");

                    category.LastActionBy = adminId;
                    category.LastAction = LastAction.Update;
                    category.LastActionTimeStamp = DateTime.UtcNow;
                }
                else
                {
                    category = new Category(
                        catDto.Name,
                        catDto.Description,
                        catDto.Status == 0 ? CategoryStatus.Active : catDto.Status,
                        adminId
                    );
                }

                product.AddCategory(category);
            }
        }

        // --- Lưu product vào DB ---
        await _productRepository.InsertAsync(product);
        await _productRepository.SaveChangesAsync();
        await transaction.CommitAsync();

        // --- Trả về DTO gọn ---
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Quantity = product.Quantity,
            SaleOff = product.SaleOff,
            Status = product.Status,
            Brand = product.Brand,
            Color = product.Color,
            Sizes = product.Sizes,
            Categories = product.ProductCategories
                                .Select(pc => new CategoryDto { Id = pc.Category.Id, Name = pc.Category.Name })
                                .ToList(),
            Images = product.Images
                            .Select(i => new ImageDto { Id = i.Id, Url = i.Url })
                            .ToList()
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto, int adminId)
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images));

        var product = products.FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException($"Product with id {id} not found");

        // --- Update fields ---
        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Quantity = dto.Quantity;
        product.SaleOff = dto.SaleOff;
        product.Status = dto.Status;

        product.LastAction = LastAction.Update;
        product.LastActionBy = adminId;
        product.LastActionTimeStamp = DateTime.UtcNow;

        if (dto.Categories != null)
        {
            var oldCategories = product.ProductCategories.ToList();
            foreach (var pc in oldCategories)
            {
                product.RemoveCategory(pc.Category);
            }

            // Thêm category mới
            foreach (var categoryId in dto.Categories)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null)
                {
                    product.AddCategory(category);
                }
            }
        }

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }
}