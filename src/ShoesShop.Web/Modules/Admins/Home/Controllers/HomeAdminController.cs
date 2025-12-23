using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.Admin.Admin.Services;
using ShoesShop.Web.Modules.Admins.Home.Dtos;

namespace ShoesShop.Web.Modules.Admins.Home.Controllers;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]

[Route("Admin")]
public class HomeAdminController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly IAdminService _adminService;

    public HomeAdminController(IWebHostEnvironment env, IAdminService adminService)
    {
        _env = env;
        _adminService = adminService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        var adminInfo = await _adminService.GetAllInfomationAsync();
        var products = adminInfo.Products;
        var adminProducts = products.Select(p => new ProductAdminDto
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

        return View("~/Modules/Admins/Home/Views/index.cshtml", adminProducts);
    }

    [HttpGet("Calendar")]
    public IActionResult Calendar()
    {
        return View("~/Modules/Admins/Views/apps-calendar.cshtml");
    }

    [HttpGet("DashboardCRM")]
    public IActionResult DashboardCRM()
    {
        return View("~/Modules/Admins/Views/dashboard-crm.cshtml");
    }

    [HttpGet("EmailInbox")]
    public IActionResult EmailInbox()
    {
        return View("~/Modules/Admins/Views/apps-email-inbox.cshtml");
    }

    [HttpGet("EmailRead")]
    public IActionResult EmailRead()
    {
        return View("~/Modules/Admins/Views/apps-email-read.cshtml");
    }
}