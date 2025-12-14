using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.User.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Web.Modules.Admins.Admin.Dtos;

namespace ShoesShop.Web.Modules.Admin.Controllers
{
    [AllowAnonymous]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IUserService _userServices;

        public AdminController(IWebHostEnvironment env, IUserService userServices)
        {
            _env = env;
            _userServices = userServices;
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Modules/Admins/Admin/Views/pages-login.cshtml");
        }

        [HttpPost("Login")]
        [ValidateModel("~/Modules/Admins/Admin/Views/pages-login.cshtml")]
        public async Task<IActionResult> Login(LoginAdminDto loginAdminModalDto, string? returnUrl = null)
        {
            var loginCommandDto = new LoginCommandDto
            {
                UserName = loginAdminModalDto.UserName,
                Password = loginAdminModalDto.Password
            };

            var result = await _userServices.LoginAsync(loginCommandDto);
            if (result == null)
            {
                return RedirectToAction("Error404", "Admin", new { message = "Invalid username or password." });
            }
            
            if (result.Role != UserAccountRole.Admin)
            {
                return RedirectToAction("Error404", "Admin", new { message = "Only admin can log in here." });
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

            var identity = new ClaimsIdentity(claims, "AdminScheme");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("AdminScheme", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Admin");
        }

        [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            return View("~/Modules/Admins/Admin/Views/pages-profile.cshtml");
        }

        [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminScheme");
            HttpContext.Session.Clear();
            return View("~/Modules/Admins/Admin/Views/pages-logout.cshtml");
        }

        [Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
        [HttpGet("Error404")]
        public IActionResult Error404(string? message)
        {
            ViewBag.Message = message;
            return View("~/Modules/Admins/Admin/Views/pages-404.cshtml", new { message });
        }
    }
}