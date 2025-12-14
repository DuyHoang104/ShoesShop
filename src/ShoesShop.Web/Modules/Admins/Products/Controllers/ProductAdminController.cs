using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.User.Categories.Enums;
using ShoesShop.Domain.Modules.User.Categories.Services;
using ShoesShop.Domain.Modules.User.Commons.Enums;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Products.Commands;
using ShoesShop.Domain.Modules.User.Products.Enums;
using ShoesShop.Domain.Modules.User.Products.Services;
using ShoesShop.Web.Modules.Admins.Products.Dtos;
using ProductUpdateDto = ShoesShop.Domain.Modules.User.Products.Commands.Dtos.ProductUpdateDto;

namespace ShoesShop.Web.Modules.Admins.Products.Controllers;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin/Products")]
public class ProductAdminController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly CloudinaryService _cloudinaryService;
    private readonly IGenericRepository<ShoesShop.Domain.Modules.User.Products.Entities.Product, int> _productRepository;
    
    public ProductAdminController(IWebHostEnvironment env, IProductService productService, ICategoryService categoryService, CloudinaryService cloudinaryService, IGenericRepository<ShoesShop.Domain.Modules.User.Products.Entities.Product, int> productRepository)
    {
        _env = env;
        _productService = productService;
        _categoryService = categoryService;
        _cloudinaryService = cloudinaryService;
        _productRepository = productRepository;
    }

    private int GetCurrentAdminId()
    {
        return int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "adminId")?.Value, out var adminId)
            ? adminId
            : 0;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllCategoriesAsync();

        var productAdminDtos = products.Select(p => new ProductAdminDto
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
            Categories = p.Categories,
            OrderDetails = p.OrderDetails,
            Carts = p.Carts,
            Images = p.Images,
            LastAction = p.LastAction,
            CreateTimeStamp = p.CreateTimeStamp
        }).ToList();

        return View("~/Modules/Admins/Products/Views/apps-ecommerce-products.cshtml", productAdminDtos);
    }

    [HttpGet("Add")]
    public async Task<IActionResult> AddProduct()
    {
        var createProductAdminDto = new CreateProductAdminDto
        {
            Name = string.Empty,
            Brand = string.Empty
        };

        var categories = await _categoryService.GetAllAsync();

        // Lọc bỏ Delete + Inactive
        createProductAdminDto.Categories = categories
            .Where(c => c.LastAction != LastAction.Delete &&
                        c.Status != CategoryStatus.Inactive)
            .ToList();

        return View("~/Modules/Admins/Products/Views/apps-projects-add.cshtml", createProductAdminDto);
    }   

    [HttpPost("Add")]
    public async Task<IActionResult> AddProduct(CreateProductAdminDto createProductAdminDto)
    {
        if (string.IsNullOrWhiteSpace(createProductAdminDto.Sizes))
        {
            ModelState.AddModelError("Sizes", "Please select at least one size.");
        }

        if (!ModelState.IsValid)
        {
            createProductAdminDto.Categories = (await _categoryService.GetAllAsync()).ToList();
            return View("~/Modules/Admins/Products/Views/apps-projects-add.cshtml", createProductAdminDto);
        }

        // Convert Sizes CSV to list nếu cần
        var sizesList = createProductAdminDto.Sizes.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        var uploadedImages = new List<string>();
        foreach (var file in createProductAdminDto.ImageFiles)
        {
            var result = await _cloudinaryService.UploadImageAsync(file, $"Products/{createProductAdminDto.Name.Replace(" ", "-")}");
            if (result != null) uploadedImages.Add(result.Value.Url);
        }

        var productDto = new CreateProductDto
        {
            Name = createProductAdminDto.Name,
            Price = createProductAdminDto.Price,
            Description = createProductAdminDto.Description ?? string.Empty,
            Quantity = createProductAdminDto.Quantity,
            SaleOff = createProductAdminDto.SaleOff,
            Status = createProductAdminDto.Status,
            Brand = createProductAdminDto.Brand,
            Color = createProductAdminDto.Color ?? string.Empty,
            Sizes = createProductAdminDto.Sizes,
            ImageUrl = uploadedImages,
            CreateBy = GetCurrentAdminId(),
            CreateTimeStamp = DateTime.UtcNow,
            LastActionBy = GetCurrentAdminId(),
            LastAction = LastAction.Create,
            LastActionTimeStamp = DateTime.UtcNow,
            Categories = createProductAdminDto.Categories.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToList(),
        };

        await _productService.CreateAsync(productDto);
        return RedirectToAction("Index", "ProductAdmin");
    }

    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var productAdminDto = new ProductAdminDto
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
            Categories = product.Categories,
            OrderDetails = product.OrderDetails,
            Carts = product.Carts,
            Images = product.Images
        };
        
        return View("~/Modules/Admins/Products/Views/apps-ecommerce-products-details.cshtml", productAdminDto);
    }

    [HttpPost("Delete/{id}")]
    [ValidateModel("Index")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return Json(new { success = false, message = "Product not found" });

        product.Status = ProductStatus.InActive;
        product.LastAction = LastAction.Delete;
        product.LastActionBy = GetCurrentAdminId();
        product.LastActionTimeStamp = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return Json(new { success = true, message = "Delete successful" });
    }

    [HttpGet("GetProduct/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var categories = await _categoryService.GetAllAsync();

        return Ok(new {
            product = product,
            allCategories = categories
        });
    }

    [HttpPost("Update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody]ProductUpdateDto productUpdateDto)
    {
        var adminId = GetCurrentAdminId();

        var result = await _productService.UpdateAsync(id, productUpdateDto, adminId);
        if (!result)
            return BadRequest(new { message = "Failed to update product" });

        return Ok(new { message = "Updated successfully" });
    }
}