using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Users.Dtos.Commands;
using ShoesShop.Domain.Users.Enums;
using ShoesShop.Domain.Users.Services;
using ShoesShop.Web.Order.Dtos.Admin.Commands;
using ShoesShop.Web.User.Dtos.Commands.Admin;

namespace ShoesShop.Web.User.Controllers.Admin;

[Area("Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/User/Views/Admin/pages-login.cshtml");
    }

    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto loginModalDto, string? returnUrl = null)
    {
        var result = await _userService.LoginAsync(new LoginCommandDto
        {
            UserName = loginModalDto.UserName,
            Password = loginModalDto.Password
        });

        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View("~/User/Views/Admin/pages-login.cshtml", loginModalDto);
        }

        if (result.Role != UserAccountRole.Admin)
        {
            ModelState.AddModelError(string.Empty, "Only administrators can log in.");
            return View("~/User/Views/Admin/pages-login.cshtml", loginModalDto);
        }

        var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, result.UserName),
                    new(ClaimTypes.Role, result.Role.ToString()),
                    new(ClaimTypes.NameIdentifier, result.ID.ToString()),
                    new(ClaimTypes.MobilePhone, result.Phone ?? string.Empty),
                    new(ClaimTypes.Email, result.Email ?? string.Empty),
                    new("AvatarUrl", result.AvatarUrl ?? string.Empty),
                    new("adminId", result.ID.ToString())
                };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "AdminScheme")
        );

        await HttpContext.SignInAsync("AdminScheme", principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("Profile")]
    public IActionResult Profile()
    {
        return View("~/User/Views/Admin/pages-profile.cshtml");
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AdminScheme");
        HttpContext.Session.Clear();
        return View("~/User/Views/Admin/pages-logout.cshtml");
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("Error404")]
    public IActionResult Error404(string? message)
    {
        ViewBag.Message = message;
        return View("~/User/Views/Admin/pages-404.cshtml", new { message });
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("Users")]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllUsersAsync();
        var userList = users.Select(user => new UserDto
        {
            ID = user.ID,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            Addresses = user.Addresses?.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList() ?? []
        }).Where(u => u.Role != UserAccountRole.Admin).ToList();

        return View("~/User/Views/Admin/apps-ecommerce-customers.cshtml", userList);
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userService.GetAllInfoUsersAsync(id);
        if (user == null)
            return NotFound();

        var userDto = new Order.Dtos.Admin.Commands.GetAllInfoUsersDto
        {
            ID = user.ID,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            Gender = user.Gender,
            AvatarUrl = user.AvatarUrl,

            Addresses = user.Addresses?.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList() ?? [],

            Orders = user.Orders?.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ShippingFee = o.ShippingFee,
                Discount = o.Discount,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                SubTotal = o.OrderDetails?.Sum(od => od.UnitPrice * od.Quantity) ?? 0m,

                OrderDetails = o.OrderDetails?.Select(od => new OrderDetailItemModalDto
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList() ?? []
            }).ToList() ?? []
        };

        return View("~/User/Views/Admin/apps-ecommerce-customers-details.cshtml", userDto);
    }

    [HttpPost("UpdateStatus/{id}")]
    [ValidateModel("Admin/Users")]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return Json(new { success = false, message = "User not found" });

        if (!Enum.TryParse<UserStatus>(newStatus, true, out var status))
            return Json(new { success = false, message = "Invalid status" });

        user.Status = status;
        await _userService.UpdateStatusAsync(user);

        return Json(new { success = true, message = "User status updated successfully" });
    }

    [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
    [HttpGet("UpdateRole/{id}")]
    [ValidateModel("Admin/Users/Details")]
    public async Task<IActionResult> UpdateRole(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return Json(new { success = false, message = "User not found" });

        if (user.Role == UserAccountRole.Admin)
            user.Role = UserAccountRole.Customer;
        else
            user.Role = UserAccountRole.Admin;

        await _userService.UpdateRoleAsync(user);

        return Json(new { success = true, message = "User role updated successfully" });
    }
}