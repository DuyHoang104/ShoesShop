using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.Products.Services;

namespace ShoesShop.Web.Modules.Home.Controllers;

public class HomeController : Controller
{
    private readonly IProductService productService;

    public HomeController(IProductService productService)
    {
        this.productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await productService.GetAllAsync();
        return View("~/Modules/Home/Views/Index.cshtml", products);
    }

    [Route("/404")]
    public IActionResult PageNotFound()
    {
        return View("~/Modules/Home/Views/PageNotFound.cshtml");
    }
}