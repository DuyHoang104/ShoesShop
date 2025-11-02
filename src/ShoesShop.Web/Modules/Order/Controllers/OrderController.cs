using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Crosscutting.Utilities.PayPal;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Orders.Dtos;
using ShoesShop.Domain.Modules.Orders.Services;
using ShoesShop.Web.Modules.Order.Dtos.Commands;

namespace ShoesShop.Web.Modules.Order.Controllers
{
    [Route("order")]
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly PaypalClient _paypalClient;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            PaypalClient paypalClient)
        {
            _orderService = orderService;
            _cartService = cartService;
            _paypalClient = paypalClient;
        }

        [HttpGet("checkout")]
        [ValidateModel("Index")]
        public async Task<IActionResult> Checkout()
        {
            var items = await _cartService.GetByUserIdAsync();
            if (items == null || items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Total = items.Sum(x => x.TotalPrice);
            ViewBag.PayPalClientId = _paypalClient.ClientId;
            var orderModelDto = new OrderModalDto
            {
                CartItems = items
            };

            return View("~/Modules/Order/Views/Index.cshtml", orderModelDto);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(OrderModalDto order)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Modules/Order/Views/Index.cshtml", order);
            }
            await _orderService.CreateOrderAsync(new OrderDto
            {
                SameAddress = order.SameAddress,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.ReceiverAddress,
                City = order.ReceiverCity,
                Country = order.ReceiverCountry,
                AddressId = order.AddressId,
                Note = order.Note,
                ShippingFee = order.ShippingCost,
                Discount = order.DiscountValue,
                PaymentMethod = Domain.Modules.Orders.Enums.PaymentMethod.Cash,
                PaymentStatus = Domain.Modules.Orders.Enums.PaymentStatus.Unpaid
            });
            await _cartService.ClearCartAsync();

            return RedirectToAction("Success");
        }


        [Authorize]
		[HttpPost("/Order/create-paypal-order")]
		public async Task<IActionResult> CreatePaypalOrder(
            CancellationToken cancellationToken,
            [FromForm(Name = "ShippingCost")] decimal shippingFee,
            [FromForm(Name = "DiscountValue")] decimal discount)
		{
            var totalAmount = await _orderService.CalculateOrderTotalAsync(discount: discount, shippingFee: shippingFee);
            var tongTien = totalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

			try
			{
				var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

				return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}

        [Authorize]
		[HttpPost("/Order/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken, [FromForm] OrderModalDto order)
		{
			try
			{
				var response = await _paypalClient.CaptureOrder(orderID);
                await _orderService.CreateOrderAsync(new OrderDto
                {
                    SameAddress = order.SameAddress,
                    ReceiverName = order.ReceiverName,
                    ReceiverPhone = order.ReceiverPhone,
                    ReceiverAddress = order.ReceiverAddress,
                    City = order.ReceiverCity,
                    Country = order.ReceiverCountry,
                    AddressId = order.AddressId,
                    Note = order.Note,
                    PaymentMethod = Domain.Modules.Orders.Enums.PaymentMethod.PayPal,
                    PaymentStatus = Domain.Modules.Orders.Enums.PaymentStatus.Paid,
                    ShippingFee = order.ShippingCost,
                    Discount = order.DiscountValue
                });
                
                await _cartService.ClearCartAsync();

                return Ok(response);
			}
			catch (Exception ex)
			{
				var error = new { ex.GetBaseException().Message };
				return BadRequest(error);
			}
		}
    }
}