using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Products.Services;

namespace ShoesShop.Web.Product.Components;

public class CategoryViewComponent : ViewComponent
{
    private readonly IProductService _productService;

    public CategoryViewComponent(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var products = await _productService.GetAllCategoriesAsync();
        products = products.Where(p => p.Status == ProductStatus.Active);

        var categoryCounts = products
            .SelectMany(p => p.Categories)
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => g.Count());

        ViewData["CategoryCounts"] = categoryCounts;

        return View("~/Product/Views/Components/CategoryView/Default.cshtml", products);
    }
}