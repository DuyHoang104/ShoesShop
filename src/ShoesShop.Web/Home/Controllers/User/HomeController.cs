using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Products.Enums;
using ShoesShop.Domain.Products.Services;

namespace ShoesShop.Web.Home.Controllers.User;

[Area("User")]
[Route("User")]
public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        products = products.Where(p => p.Status == ProductStatus.Active).ToList();
        return View("~/Home/Views/User/Index.cshtml", products);
    }

    [HttpGet("404")]
    [Route("/404")]
    public IActionResult PageNotFound()
    {
        return View("~/Home/Views/User/PageNotFound.cshtml");
    }
}