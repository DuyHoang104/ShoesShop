using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.User.Carts.Dtos;
using ShoesShop.Domain.Modules.User.Carts.Services;

namespace ShoesShop.Web.Modules.Users.Cart.Controllers;
[Authorize(AuthenticationSchemes = "UserScheme", Roles = "Customer")]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return 0;

        return int.Parse(userIdClaim);
    }

    public async Task<IActionResult> Index()
    {
        var items = await _cartService.GetByUserIdAsync(GetUserIdFromClaims());
        ViewBag.Total = items.Sum(x => x.TotalPrice);
        return View("~/Modules/Users/Cart/Views/Cart.cshtml", items);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> AddToCart(int id, int quantity, string size)
    {
        if (string.IsNullOrEmpty(size) || quantity <= 0)
        {
            ModelState.AddModelError("", "Please select a valid size and quantity.");
            return View("Detail");
        }

        await _cartService.AddToCartAsync(id, quantity, size, GetUserIdFromClaims());
        return RedirectToAction("Index", "Cart");
    }

    public async Task<IActionResult> RemoveFromCart(int productId, string size)
    {
        await _cartService.RemoveFromCartAsync(productId, size, GetUserIdFromClaims());
        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCart(List<CartDto> items)
    {
        if (items == null || items.Count == 0)
            return Content("No items to update.");
        await _cartService.UpdateCartAsync(items, GetUserIdFromClaims());
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(GetUserIdFromClaims());
        var emptyCart = new List<CartDto>();

        return RedirectToAction("Index", "Cart", emptyCart);
    }
}