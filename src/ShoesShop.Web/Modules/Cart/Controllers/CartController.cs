using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.Carts.Dtos;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Web.Modules.Order.Dtos.Commands;

namespace ShoesShop.Web.Modules.Cart.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IWebHostEnvironment _env;

        public CartController(ICartService cartService, IWebHostEnvironment env)
        {
            _cartService = cartService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _cartService.GetByUserIdAsync();
            ViewBag.Total = items.Sum(x => x.TotalPrice);
            return View("~/Modules/Cart/Views/Cart.cshtml", items);
        }

        [ValidateModel("Detail")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity, string size)
        {
            if (string.IsNullOrEmpty(size) || quantity <= 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn size và số lượng hợp lệ.");
                return View("Detail");
            }

            await _cartService.AddToCartAsync(id, quantity, size);
            return RedirectToAction("Index", "Cart");
        }

        public async Task<IActionResult> RemoveFromCart(int productId, string size)
        {
            await _cartService.RemoveFromCartAsync(productId, size);
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(List<CartItemDto> items)
        {
            if (items == null || items.Count == 0)
                return Content("❌ Không nhận được dữ liệu");
            await _cartService.UpdateCartAsync(items);
            return RedirectToAction("Index");
        }

        // [HttpPost]
        // public async Task<IActionResult> AddShipping(string Country, string City, string Address)
        // {
        //     await _cartService.AddShippingAsync(Country, City, Address);
        //     return RedirectToAction("Index");
        // }
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync();
            var emptyCart = new List<CartItemDto>();

            return RedirectToAction("Index", "Cart", emptyCart);
        }
    }
}