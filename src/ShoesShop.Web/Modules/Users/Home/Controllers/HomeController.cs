using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.User.Products.Enums;
using ShoesShop.Domain.Modules.User.Products.Services;

namespace ShoesShop.Web.Modules.Home.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        products = products.Where(p => p.Status == ProductStatus.Active).ToList();
        return View("~/Modules/Users/Home/Views/Index.cshtml", products);
    }

    [Route("/404")]
    public IActionResult PageNotFound()
    {
        return View("~/Modules/Users/Home/Views/PageNotFound.cshtml");
    }
}