using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Categories.Enums;
using ShoesShop.Domain.Categories.Services;
using ShoesShop.Domain.Commons.Enums;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Products.Commands.Dtos;
using ShoesShop.Domain.Products.Services;
using ShoesShop.Web.Product.Model.Admin;
using ProductUpdateDto = ShoesShop.Domain.Products.Commands.Dtos.ProductUpdateDto;

namespace ShoesShop.Web.Product.Controllers.Admin;

[Area("Admin")]
[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin/Products")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly CloudinaryService _cloudinaryService;
    private readonly IGenericRepository<Domain.Products.Entities.Product, int> _productRepository;

    public ProductController(IProductService productService, ICategoryService categoryService, CloudinaryService cloudinaryService, IGenericRepository<Domain.Products.Entities.Product, int> productRepository)
    {
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
            StockStatus = p.StockStatus,
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

        return View("~/Product/Views/Admin/apps-ecommerce-products.cshtml", productAdminDtos);
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

        createProductAdminDto.Categories = categories
            .Where(c => c.LastAction != LastAction.Delete &&
                        c.Status != CategoryStatus.Inactive)
            .ToList();

        return View("~/Product/Views/Admin/apps-projects-add.cshtml", createProductAdminDto);
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add(CreateProductAdminDto dto)
    {
        if (!ModelState.IsValid)
        {
            dto.Categories = (await _categoryService.GetAllAsync()).ToList();
            return View("~/Product/Views/Admin/apps-projects-add.cshtml", dto);
        }

        await _productService.CreateAsync(new CreateProductDto
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description ?? string.Empty,
            Quantity = dto.Quantity,
            SaleOff = dto.SaleOff,
            Brand = dto.Brand,
            Color = dto.Color ?? string.Empty,
            Sizes = dto.Sizes,
            ImageFiles = dto.ImageFiles,
            CreateBy = GetCurrentAdminId(),
            Categories = dto.Categories.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToList()
        });

        return RedirectToAction("Index", "Product");
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
            StockStatus = product.StockStatus,
            Status = product.Status,
            Brand = product.Brand,
            Color = product.Color,
            Sizes = product.Sizes,
            Categories = product.Categories,
            OrderDetails = product.OrderDetails,
            Carts = product.Carts,
            Images = product.Images,
            Reviews = product.Reviews,
        };

        return View("~/Product/Views/Admin/apps-ecommerce-products-details.cshtml", productAdminDto);
    }

    [HttpPost("Delete/{id}")]
    [ValidateModel("Index")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return Json(new { success = false, message = "Product not found" });

        await _productService.DeleteAsync(product.Id);
        return Json(new { success = true, message = "Delete successful" });
    }

    [HttpGet("GetProduct/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var categories = await _categoryService.GetAllAsync();

        return Ok(new
        {
            product = product,
            allCategories = categories
        });
    }

    [HttpPost("Update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto productUpdateDto)
    {
        var adminId = GetCurrentAdminId();

        var result = await _productService.UpdateAsync(id, productUpdateDto, adminId);
        if (!result)
            return BadRequest(new { message = "Failed to update product" });

        return Ok(new { message = "Updated successfully" });
    }
}