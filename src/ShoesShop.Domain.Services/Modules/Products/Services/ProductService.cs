using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Categories.Dtos;
using ShoesShop.Domain.Categories.Entities;
using ShoesShop.Domain.Categories.Enums;
using ShoesShop.Domain.Commons.Enums;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Users.Entities;
using ShoesShop.Domain.Products.Commands;
using ShoesShop.Domain.Products.Commands.Dtos;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Products.Services;
using ShoesShop.Domain.Shares.Image.Dtos;
using ShoesShop.Domain.Shares.Review.Dtos;
using ShoesShop.Domain.Shares.Review.Entity;
using ShoesShop.Domain.Users.Dtos;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Products.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Product, int> _productRepository;
    private readonly IGenericRepository<Category, int> _categoryRepository;
    private readonly IGenericRepository<Review, int> _reviewRepository;
    private readonly IGenericRepository<User, int> _userRepository;
    private readonly CloudinaryService _cloudinaryService;

    public ProductService(IGenericRepository<Product, int> productRepository, IGenericRepository<Category, int> categoryRepository,
        CloudinaryService cloudinaryService, IUnitOfWork unitOfWork, IGenericRepository<Review, int> reviewRepository,
        IGenericRepository<User, int> userRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
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
            Status = p.Status,
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
                .Where(c => c.Status == CategoryStatus.Active)
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

    public async Task<ProductDto?> GetByIdAsync(int productId)
    {
        var product = (await _productRepository.GetAllAsync(
            p => p.Id == productId,
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images)
        )).FirstOrDefault();

        if (product == null)
            return null;

        var reviews = await _reviewRepository.GetAllAsync(
            r => r.ParentId != null && r.Metadata != null
        );

        var productReviews = reviews
            .Select(r =>
            {
                try
                {
                    var json = r.Metadata switch
                    {
                        string s => s,
                        JsonElement je => je.GetRawText(),
                        _ => JsonSerializer.Serialize(r.Metadata)
                    };

                    var meta = JsonSerializer.Deserialize<ProductReviewMetadata>(json);
                    if (meta == null || meta.ProductId != productId)
                        return null;

                    return new ProductReviewDto
                    {
                        OrderDetailId = meta.OrderDetailId,
                        ProductId = meta.ProductId,
                        ProductComment = r.Comment,
                        ProductRating = r.Rating,
                        UserId = meta.UserId,
                        ImageUrls = meta.Images?.Select(i => i.Url).ToList() ?? [],
                        ImageId = null
                    };
                }
                catch
                {
                    return null;
                }
            })
            .Where(pr => pr != null)
            .ToList()!;

        var userId = productReviews.FirstOrDefault()?.UserId;

        var user = userId == null
            ? null
            : (await _userRepository.GetAllAsync(
                u => u.Id == userId.Value,
                include: q => q.Include(u => u.Images)
            )).FirstOrDefault();

        var userDto = user == null
            ? new UserDto
            {
                ID = 0,
                UserName = "Unknown",
                Email = "Unknown",
                AvatarUrl = null
            }
            : new UserDto
            {
                ID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AvatarUrl = user.Images?
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefault()?.Url
            };

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
                    Name = pc.Category.Name,
                    Status = pc.Category.Status
                }).ToList() ?? [],

            Images = product.Images?
                .Select(i => new ImageDto
                {
                    Id = i.Id,
                    Url = i.Url
                }).ToList() ?? [],

            Reviews = productReviews.Select(pr => new ReviewDto
            {
                Rating = pr.ProductRating,
                Comment = pr.ProductComment ?? string.Empty,
                ProductReview = [pr],
                User = userDto
            }).ToList()
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
        var uploadedImages = new List<(string Url, string PublicId)>();

        try
        {
            if (createProductDto.ImageFiles?.Any() == true)
            {
                foreach (var file in createProductDto.ImageFiles)
                {
                    var image = await _cloudinaryService.UploadImageAsync(
                        file,
                        $"Products/{createProductDto.Name.Replace(" ", "-")}"
                    ) ?? throw new InvalidOperationException("Upload product image failed");

                    uploadedImages.Add((image.Url, image.FileName));
                }
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Description = createProductDto.Description ?? string.Empty,
                Quantity = createProductDto.Quantity,
                SaleOff = createProductDto.SaleOff,
                Status = ProductStatus.Active,
                Brand = createProductDto.Brand,
                Color = createProductDto.Color ?? string.Empty,
                Sizes = createProductDto.Sizes,
                CreateBy = adminId,
                CreateTimeStamp = DateTime.UtcNow,
                LastActionBy = adminId,
                LastAction = LastAction.Create,
                LastActionTimeStamp = DateTime.UtcNow
            };

            foreach (var img in uploadedImages)
            {
                product.AddImage(img.Url, img.PublicId);
            }

            if (createProductDto.Categories?.Any() == true)
            {
                foreach (var catDto in createProductDto.Categories)
                {
                    Category category;

                    if (catDto.Id > 0)
                    {
                        category = await _categoryRepository.GetByIdAsync(catDto.Id)
                            ?? throw new InvalidOperationException($"Category with ID {catDto.Id} not found.");
                    }
                    else
                    {
                        category = new Category(
                            catDto.Name,
                            catDto.Description,
                            catDto.Status == 0 ? CategoryStatus.Active : catDto.Status
                        )
                        {
                            CreateBy = adminId,
                            CreateTimeStamp = DateTime.UtcNow,
                            LastActionBy = adminId,
                            LastAction = LastAction.Create
                        };
                    }

                    product.AddCategory(category);
                }
            }

            await _productRepository.InsertAsync(product);
            await _productRepository.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

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
                    .Select(pc => new CategoryDto
                    {
                        Id = pc.Category.Id,
                        Name = pc.Category.Name
                    }).ToList(),
                Images = product.Images
                    .Select(i => new ImageDto
                    {
                        Id = i.Id,
                        Url = i.Url
                    }).ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();

            foreach (var img in uploadedImages)
            {
                await _cloudinaryService.DeleteImageAsync(img.PublicId);
            }

            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var products = await _productRepository.GetAllAsync(
                predicate: p => p.Id == id,
                include: q => q.Include(p => p.Images)
            );

            var product = products.FirstOrDefault();
            if (product == null)
                return false;

            foreach (var image in product.Images)
            {
                if (!string.IsNullOrWhiteSpace(image.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(image.PublicId);
                }
            }

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto, int adminId)
    {
        var products = await _productRepository.GetAllAsync(
            include: q => q.Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                        .Include(p => p.Images));

        var product = products.FirstOrDefault(p => p.Id == id)
            ?? throw new NotFoundException($"Product with id {id} not found");

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