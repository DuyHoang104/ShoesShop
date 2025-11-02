using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.Users.Enums;
using ShoesShop.Domain.Modules.Users.Services;
using ShoesShop.Web.Modules.User.Dtos.Commands;

namespace ShoesShop.Web.Modules.User.Controllers;

[Route("User")]
[AllowAnonymous]

public class UserController : Controller
{
    private readonly IUserService _userServices;
    private readonly IWebHostEnvironment _env;
    private readonly CloudinaryService _cloudinaryService;
    const string SessionKey = "UserId";

    public UserController(IUserService userServices, IWebHostEnvironment env, CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
        _userServices = userServices;
        _env = env;
    }

    [HttpGet("")]
    public IActionResult Index(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Modules/User/Views/Index.cshtml");
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterModalDto registerModalDto)
    {
        string? imageUrl = null;

        if (registerModalDto.AvatarUrl != null)
        {
            try
            {
                var result = await _cloudinaryService.UploadImageAsync(
                    registerModalDto.AvatarUrl, 
                    $"Users/{registerModalDto.UserName.Replace(" ", "-")}"
                );

                if (result != null)
                    imageUrl = result.Value.Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading image: " + ex.Message);
                ModelState.AddModelError("", "Error uploading avatar image");
            }
        }

        var registerCommandDto = new RegisterCommandDto
        {
            UserName = registerModalDto.UserName,
            Password = registerModalDto.Password,
            ConfirmPassword = registerModalDto.ConfirmPassword,
            DateOfBirth = registerModalDto.DateOfBirth,
            Email = registerModalDto.Email,
            Phone = registerModalDto.Phone,
            Gender = registerModalDto.Gender,
            Addresses = registerModalDto.Addresses,
            AvatarUrl = imageUrl,
            Role = UserAccountRole.Customer
        };

        var resultUser = await _userServices.RegisterAsync(registerCommandDto);

        if (resultUser != null)
            return RedirectToAction("Index", "Home");

        return View("~/Modules/User/Views/Index.cshtml", registerModalDto);
    }

    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View("~/Modules/User/Views/Index.cshtml");
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModalDto loginModalDto, string? returnUrl = null)
    {
        var loginCommandDto = new LoginCommandDto
        {
            UserName = loginModalDto.UserName,
            Password = loginModalDto.Password
        };

        var result = await _userServices.LoginAsync(loginCommandDto);

        if (result == null)
        {
            ModelState.AddModelError("", "Invalid username or password");
            return View("~/Modules/User/Views/Index.cshtml", loginModalDto);
        }

        if (result.Status == Domain.Modules.Users.Enums.UserStatus.Inactive)
        {
            ModelState.AddModelError("", "Your account is inactive. Please contact support.");
            return View("~/Modules/User/Views/Index.cshtml", loginModalDto);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, result.UserName),
            new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.Role, result.Role.ToString()),
            new(ClaimTypes.NameIdentifier, result.ID.ToString()),
            new("userId", result.ID.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "User");
    }
}