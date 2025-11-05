using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Crosscutting.Utilities.PayPal;
using ShoesShop.Crosscutting.Utilities.VNpay;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Orders.Dtos;
using ShoesShop.Domain.Modules.Orders.Services;
using ShoesShop.Web.Modules.Order.Dtos.Commands;

namespace ShoesShop.Web.Modules.Order.Controllers
{
    [Route("Order")]
    [Authorize(Roles = "Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly PaypalClient _paypalClient;
        private readonly IVnPayService _vnPayService;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            PaypalClient paypalClient,
            IVnPayService vnPayService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _paypalClient = paypalClient;
            _vnPayService = vnPayService;
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
        [ValidateModel("Index")]
        public async Task<IActionResult> Checkout(OrderModalDto order, string payment = "COD")
        {
            if (payment == "VNPAY")
            {
                HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(order));

                var model = new VnPayRequestModel
                {
                    OrderId = new Random().Next(1000, 9999),
                    FullName = order.ReceiverName ?? "Khách hàng",
                    Description = $"Thanh toán đơn hàng {order.ReceiverName ?? "Khách hàng"}, {order.ReceiverPhone ?? "Không có số điện thoại"}",
                    Amount = await _orderService.CalculateOrderTotalAsync(order.ShippingCost, order.DiscountValue),
                    CreatedDate = DateTime.Now
                };


                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, model));
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

        [HttpGet("/Order/PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = "Payment failed!";
                return RedirectToAction("Fail");
            }
            var tempOrderJson = HttpContext.Session.GetString("TempOrder");
            if (string.IsNullOrEmpty(tempOrderJson))
            {
                TempData["Message"] = "Session expired or invalid order data.";
                return RedirectToAction("Fail");
            }

            var order = JsonConvert.DeserializeObject<OrderModalDto>(tempOrderJson);

            // ✅ Tạo order thực tế trong DB
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
                PaymentMethod = Domain.Modules.Orders.Enums.PaymentMethod.VnPayCard,
                PaymentStatus = Domain.Modules.Orders.Enums.PaymentStatus.Paid,
                ShippingFee = order.ShippingCost,
                Discount = order.DiscountValue
            });

            // ✅ Xóa giỏ hàng và session tạm
            await _cartService.ClearCartAsync();
            HttpContext.Session.Remove("TempOrder");
            TempData["Message"] = "Payment successful!";
            return RedirectToAction("Success");
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("~/Modules/Order/Views/Success.cshtml");
        }

        [HttpGet("fail")]
        public IActionResult Fail(string vnp_ResponseCode)
        {
            ViewBag.ResponseCode = vnp_ResponseCode;
            ViewBag.Message = TempData["Message"];

            return View("~/Modules/Order/Views/Fail.cshtml");
        }
    }
}