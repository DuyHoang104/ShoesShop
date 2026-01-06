using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Orders.Services;
using ShoesShop.Domain.Users.Services;
using ShoesShop.Web.Home.Dtos.Admin;

namespace ShoesShop.Web.Home.Controllers.Admin;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin")]
[Area("Admin")]
public class HomeController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IOrderService _orderService;

    public HomeController(IAdminService adminService, IOrderService orderService)
    {
        _adminService = adminService;
        _orderService = orderService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? returnUrl = null)
    {
        var adminInfo = await _adminService.GetAllInfomationAsync();

        var adminProducts = adminInfo.Products.Select(p => new ProductDto
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
        ViewData["OrdersByDate"] = adminInfo.OrdersByDate;
        ViewData["TodayRevenue"] = adminInfo.TodayRevenue;
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["OrdersByLocation"] = adminInfo.OrdersByLocation;

        return View("~/Home/Views/Admin/index.cshtml", adminProducts);
    }
}