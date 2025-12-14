using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.User.Categories.Services;
using ShoesShop.Domain.Modules.User.Products.Services;
using ShoesShop.Web.Modules.Product.Dtos;

namespace ShoesShop.Web.Modules.Users.Product.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(SearchModalDto searchDto)
    {
        var products = await _productService.GetAllCategoriesAsync();

        if (searchDto.CategoryId.HasValue)
            products = products.Where(p => p.Categories.Any(c => c.Id == searchDto.CategoryId.Value)).ToList();

        if (!string.IsNullOrEmpty(searchDto.Brand))
            products = products.Where(p => p.Brand.Equals(searchDto.Brand, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(searchDto.Color))
            products = products
                .Where(p =>
                    !string.IsNullOrEmpty(p.Color) &&
                    p.Color.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Any(c => c.Trim().Equals(searchDto.Color, StringComparison.OrdinalIgnoreCase)))
                .ToList();

        if (searchDto.MinPrice.HasValue && searchDto.MaxPrice.HasValue)
            products = products.Where(p => p.Price >= searchDto.MinPrice && p.Price <= searchDto.MaxPrice).ToList();

        products = searchDto.SortBy switch
        {
            "name" => products.OrderBy(p => p.Name).ToList(),
            "price" => products.OrderBy(p => p.Price).ToList(),
            "brand" => products.OrderBy(p => p.Brand).ToList(),
            _ => products
        };

        if (searchDto.Sizes != null && searchDto.Sizes.Count != 0)
        {
            products = products
                .Where(p =>
                    !string.IsNullOrEmpty(p.Sizes) &&
                    searchDto.Sizes.All(selectedSize =>
                        p.Sizes
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .Contains(selectedSize)
                    )
                ).ToList();
        }

        ViewBag.SelectedSizes = searchDto.Sizes ?? [];
        return View("~/Modules/Users/Product/Views/Index.cshtml", products);
    }

    [HttpPost]
    public async Task<IActionResult> Search(string? query)
    {
        var products = await _productService.SearchAsync(query);

        return View("~/Modules/Users/Product/Views/Index.cshtml", products);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return View("~/Modules/Users/Home/Views/PageNotFound.cshtml");
        }

        return View("~/Modules/Users/Product/Views/Detail.cshtml", product);
    }
}
