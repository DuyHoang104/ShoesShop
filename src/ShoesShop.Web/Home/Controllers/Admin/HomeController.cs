using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Users.Services;
using ShoesShop.Web.Home.Dtos.Admin;

namespace ShoesShop.Web.Home.Controllers.Admin;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin")]
[Area("Admin")]
public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IAdminService _adminService;

    public HomeController(IWebHostEnvironment env, IAdminService adminService)
    {
        _env = env;
        _adminService = adminService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        var adminInfo = await _adminService.GetAllInfomationAsync();
        var products = adminInfo.Products;
        var adminProducts = products.Select(p => new ProductDto
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
            Categories = p.Categories ?? [],
            Images = p.Images ?? []
        }).ToList();

        ViewData["UserCount"] = adminInfo.UserCount;
        ViewData["OrderCount"] = adminInfo.OrderCount;
        ViewData["OrderRevenue"] = adminInfo.OrderRevenue;

        ViewData["ReturnUrl"] = returnUrl;

        return View("~/Home/Views/Admin/index.cshtml", adminProducts);
    }

    [HttpGet("dashboard-crm")]
    public IActionResult DashboardCRM()
    {
        return View("~/Home/Views/Admin/dashboard-crm.cshtml");
    }
}