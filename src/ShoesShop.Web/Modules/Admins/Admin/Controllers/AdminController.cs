using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.User.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Web.Modules.Admins.Admin.Dtos;

namespace ShoesShop.Web.Modules.Admins.Admin.Controllers
{
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userServices;

        public AdminController(IUserService userServices)
        {
            _userServices = userServices;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Modules/Admins/Admin/Views/pages-login.cshtml");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginAdminDto loginAdminModalDto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Modules/Admins/Admin/Views/pages-login.cshtml", loginAdminModalDto);
            }

            var result = await _userServices.LoginAsync(new LoginCommandDto
            {
                UserName = loginAdminModalDto.UserName,
                Password = loginAdminModalDto.Password
            });

            if (result == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View("~/Modules/Admins/Admin/Views/pages-login.cshtml", loginAdminModalDto);
            }

            if (result.Role != UserAccountRole.Admin)
            {
                ModelState.AddModelError(string.Empty, "Only administrators can log in.");
                return View("~/Modules/Admins/Admin/Views/pages-login.cshtml", loginAdminModalDto);
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