using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Categories.Enums;
using ShoesShop.Domain.Categories.Services;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Products.Services;
using ShoesShop.Web.Product.Model.User;

namespace ShoesShop.Web.Product.Controllers.User;

[Area("User")]
[Route("User/Products")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(SearchModal searchDto)
    {
        var products = (await _productService.GetAllCategoriesAsync())
            .Where(p => p.Status == ProductStatus.Active)
            .AsQueryable();

        if (searchDto.CategoryId.HasValue)
        {
            products = products.Where(p =>
                p.Categories.Any(c =>
                    c.Id == searchDto.CategoryId.Value &&
                    c.Status == CategoryStatus.Active
                ));
        }

        if (!string.IsNullOrEmpty(searchDto.Brand))
        {
            products = products.Where(p =>
                p.Brand.Equals(searchDto.Brand, StringComparison.OrdinalIgnoreCase)
                );
        }

        if (!string.IsNullOrEmpty(searchDto.Color))
        {
            products = products.Where(p =>
                !string.IsNullOrEmpty(p.Color) &&
                p.Color.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Any(c => c.Trim().Equals(searchDto.Color, StringComparison.OrdinalIgnoreCase)));
        }

        if (searchDto.MinPrice.HasValue && searchDto.MaxPrice.HasValue)
        {
            products = products.Where(p =>
                p.Price >= searchDto.MinPrice &&
                p.Price <= searchDto.MaxPrice);
        }

        products = searchDto.SortBy switch
        {
            "name" => products.OrderBy(p => p.Name),
            "price" => products.OrderBy(p => p.Price),
            "brand" => products.OrderBy(p => p.Brand),
            _ => products
        };

        if (searchDto.Sizes != null && searchDto.Sizes.Count != 0)
        {
            products = products.Where(p =>
                !string.IsNullOrEmpty(p.Sizes) &&
                searchDto.Sizes.All(selectedSize =>
                    p.Sizes
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Contains(selectedSize)
                ));
        }

        ViewBag.SelectedSizes = searchDto.Sizes ?? [];
        return View("~/Product/Views/User/Index.cshtml", products.ToList());
    }

    [HttpPost("Search")]
    public async Task<IActionResult> Search(string? query)
    {
        var products = await _productService.SearchAsync(query);

        return View("~/Product/Views/User/Index.cshtml", products);
    }

    [HttpGet("Detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return View("~/Modules/Users/Home/Views/PageNotFound.cshtml");
        }

        product.Categories = product.Categories
            .Where(c => c.Status == CategoryStatus.Active)
            .ToList();

        return View("~/Product/Views/User/Detail.cshtml", product);
    }
}