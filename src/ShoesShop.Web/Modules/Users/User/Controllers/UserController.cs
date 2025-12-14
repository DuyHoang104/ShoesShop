using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.User.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Web.Modules.Users.User.Dtos.Commands;

namespace ShoesShop.Web.Modules.User.Controllers;

[Route("User")]
[AllowAnonymous]
public class UserController : Controller
{
    private readonly IUserService _userServices;
    private readonly CloudinaryService _cloudinaryService;

    public UserController(IUserService userServices, CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
        _userServices = userServices;
    }

    [HttpGet("")]
    [ValidateModel("Index")]
    public IActionResult Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Modules/Users/User/Views/index.cshtml");
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterModalDto registerModalDto)
    {
        string? imageUrl = null;

        if (registerModalDto.AvatarUrl != null)
        {
            try
            {
                var image = await _cloudinaryService.UploadImageAsync(
                    registerModalDto.AvatarUrl,
                    $"Users/{registerModalDto.UserName.Replace(" ", "-")}"
                );

                if (image != null)
                    imageUrl = image.Value.Url;
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error uploading avatar image." });
            }
        }

        var registerCommandDto = new RegisterCommandDto
        {
            UserName = registerModalDto.UserName,
            Password = registerModalDto.Password,
            DateOfBirth = registerModalDto.DateOfBirth,
            Email = registerModalDto.Email,
            Phone = registerModalDto.Phone,
            Gender = registerModalDto.Gender,
            Addresses = registerModalDto.Addresses,
            AvatarUrl = imageUrl,
            Role = UserAccountRole.Customer
        };

        var result = await _userServices.RegisterAsync(registerCommandDto);

        if (result != null)
        {
            return Json(new { success = true, message = "Registration successful!" });
        }

        return Json(new { success = false, message = "Registration failed. Please check your data." });
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Modules/Users/User/Views/index.cshtml");
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModalDto loginModalDto)
    {
        var loginCommandDto = new LoginCommandDto
        {
            UserName = loginModalDto.UserName,
            Password = loginModalDto.Password
        };

        var result = await _userServices.LoginAsync(loginCommandDto);

        if (result == null)
            return Json(new { success = false, message = "Invalid login attempt." });

        if (result.Role != UserAccountRole.Customer)
            return Json(new { success = false, message = "Only customers can log in here." });

        if (result.Status >= UserStatus.Banned)
            return Json(new { success = false, message = "Your account has been banned." });

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.UserName),
            new(ClaimTypes.Role, result.Role.ToString()),
            new(ClaimTypes.NameIdentifier, result.ID.ToString()),
            new("userId", result.ID.ToString())
        };

        var identity = new ClaimsIdentity(claims, "UserCookie");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("UserScheme", principal);

        return Json(new { success = true, message = "Login successful!" });
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("UserScheme");
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");   
    }

    [HttpGet("Error404")]
    public IActionResult Error404(string? message)
    {
        ViewBag.Message = message;
        Response.StatusCode = 404;
        return View("~/Modules/Users/Shares/Views/pages-404.cshtml", new { message });
    }
}