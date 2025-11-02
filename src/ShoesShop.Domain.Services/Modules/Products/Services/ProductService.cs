using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Modules.Categories.Dtos;
using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Categories.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Products.Dtos;
using ShoesShop.Domain.Modules.Products.Dtos.Commands;
using ShoesShop.Domain.Modules.Products.Entities;
using ShoesShop.Domain.Modules.Products.Services;
using ShoesShop.Domain.Modules.Shares.Dtos;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Products.Services
{
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
                Quantity = p.Quantity,
                SaleOff = p.SaleOff,
                Status = p.Status,
                Brand = p.Brand,
                Color = p.Color,
                Sizes = p.Sizes,
                Images = p.Images.Select(i => new ImageUploadDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList() ?? []
            });
        }
        
        public async Task<ProductDto> CreateAsync(CreateProductDto createProductDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Description = createProductDto.Description,
                Quantity = createProductDto.Quantity,
                SaleOff = createProductDto.SaleOff,
                Status = createProductDto.Status,
                Brand = createProductDto.Brand,
                Color = createProductDto.Color,
                Sizes = createProductDto.Sizes
            };

            if (createProductDto.ImageUrl != null && createProductDto.ImageUrl.Count != 0)
            {
                foreach (var url in createProductDto.ImageUrl)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        product.AddImage(url.Trim());
                    }
                }
            }

            if (createProductDto.Categories != null && createProductDto.Categories.Count != 0)
            {
                foreach (var categoryDto in createProductDto.Categories)
                {
                    Category category;

                    if (categoryDto.Id > 0)
                    {
                        category = await _categoryRepository.GetByIdAsync(categoryDto.Id)
                            ?? throw new InvalidOperationException($"Category with ID {categoryDto.Id} not found.");
                    }
                    else
                    {
                        category = new Category(categoryDto.Name, categoryDto.Description);
                    }

                    product.AddCategory(category);
                }
            }

            await _productRepository.InsertAsync(product);
            await _productRepository.SaveChangesAsync();

            await transaction.CommitAsync();

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
                Categories = product.ProductCategories.Select(pc => new CategoryDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToList() ?? new List<CategoryDto>(),

                Images = product.Images.Select(i => new ImageUploadDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList() ?? new List<ImageUploadDto>()
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllCategoriesAsync()
        {
            var products = await _productRepository.GetAllAsync(
            include: q =>   q.Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                            .Include(p => p.Images));

            var productDtos = new List<ProductDto>();

            foreach (var p in products)
            {
                var category = p.ProductCategories?
                                    .Select(pc => pc.CategoryId)
                                    .ToList() ?? [];
                var categories = (await _categoryService.GetListByIdAsync(category)).ToList();

                productDtos.Add(new ProductDto
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
                    Categories = categories.ToList() ?? [],
                    Images = p.Images.Select(i => new ImageUploadDto
                    {
                        Id = i.Id,
                        Url = i.Url
                    }).ToList() ?? []
                });
            }

            return productDtos;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var products = await _productRepository.GetAllAsync(
                include: q => q.Include(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                            .Include(p => p.Images));

            var product = products.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Product with id {id} not found");

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

                Images = product.Images.Select(i => new ImageUploadDto
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
                
                    Images = p.Images.Select(i => new ImageUploadDto
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
                Images = p.Images.Select(i => new ImageUploadDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList() ?? []
            });

            return result;
        }
    }
}