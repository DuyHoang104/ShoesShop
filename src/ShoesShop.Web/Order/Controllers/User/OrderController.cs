using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Crosscutting.Utilities.PayPal;
using ShoesShop.Crosscutting.Utilities.VNpay;
using ShoesShop.Domain.Carts.Services;
using ShoesShop.Domain.Orders.Dtos.Commands;
using ShoesShop.Domain.Orders.Enums;
using ShoesShop.Domain.Orders.Services;
using ShoesShop.Domain.Shares.Messages.Services;
using ShoesShop.Domain.Users.Services;
using ShoesShop.Web.Modules.Users.Order.Dtos;
using ShoesShop.Web.Modules.Users.Order.Dtos.Commands;
using ShoesShop.Web.Order.Dtos.User.Commands;

namespace ShoesShop.Web.Order.Controllers.User;


[Authorize(AuthenticationSchemes = "UserScheme", Roles = "Customer")]
[Route("User/Order")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly PaypalClient _paypalClient;
    private readonly IVnPayService _vnPayService;
    private readonly IChatService _chatService;
    private readonly IUserService _userService;

    public OrderController(IOrderService orderService, ICartService cartService, PaypalClient paypalClient, IVnPayService vnPayService, IChatService chatService, IUserService userService)
    {
        _orderService = orderService;
        _cartService = cartService;
        _paypalClient = paypalClient;
        _vnPayService = vnPayService;
        _chatService = chatService;
        _userService = userService;
    }

    private int GetCurrentUserId()
    {
        return int.TryParse(User.FindFirst("userId")?.Value, out var userId) ? userId : 0;
    }

    [HttpGet("index")]
    [ValidateModel("Index")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrderAsync(GetCurrentUserId());
        var ordermodalDto = new List<OrderModalDto>(
            orders.Select(o => new OrderModalDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemModalDto
                {
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal,
                    Size = od.Size,
                    ProductImage = od.ProductImage,
                }).ToList()
            })
        );
        return View("~/Order/Views/User/Index.cshtml", ordermodalDto);
    }

    [HttpGet("checkout")]
    [ValidateModel("Checkout")]
    public async Task<IActionResult> Checkout()
    {
        var items = await _cartService.GetByUserIdAsync(GetCurrentUserId());
        if (items == null || items.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        ViewBag.Total = items.Sum(x => x.TotalPrice);
        ViewBag.PayPalClientId = _paypalClient.ClientId;
        var orderModelDto = new OrderCheckoutModalDto
        {
            Carts = items
        };

        return View("~/Order/Views/User/Checkout.cshtml", orderModelDto);
    }

    [HttpPost("checkout")]
    [ValidateModel("Checkout")]
    public async Task<IActionResult> Checkout(OrderCheckoutModalDto order, string payment = "COD")
    {
        if (payment == "VNPAY")
        {
            HttpContext.Session.SetString("TempOrder", JsonConvert.SerializeObject(order));

            var model = new VnPayRequestModel
            {
                OrderId = new Random().Next(1000, 9999),
                FullName = order.ReceiverName ?? "Customer",
                Description = $"Payment for the order {order.ReceiverName ?? "Customer"}, {order.ReceiverPhone ?? "No phone number"}",
                Amount = await _orderService.CalculateOrderTotalAsync(GetCurrentUserId(), order.ShippingCost, order.DiscountValue),
                CreatedDate = DateTime.Now
            };

            return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, model));
        }

        var orderCreated = await _orderService.CreateOrderAsync(new OrderCheckoutDto
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
            PaymentMethod = PaymentMethod.Cash,
            PaymentStatus = PaymentStatus.Unpaid
        }, GetCurrentUserId());

        await _cartService.ClearCartAsync(GetCurrentUserId());

        return RedirectToAction("Success", new { orderId = orderCreated.Id });
    }

    [HttpPost("/Order/create-paypal-order")]
    public async Task<IActionResult> CreatePaypalOrder(
        CancellationToken cancellationToken,
        [FromForm(Name = "ShippingCost")] decimal shippingFee,
        [FromForm(Name = "DiscountValue")] decimal discount)
    {
        var totalAmount = await _orderService.CalculateOrderTotalAsync(GetCurrentUserId(), shippingFee, discount);
        var amount = totalAmount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        var currency = "USD";
        var referenceOrderId = "DH" + DateTime.Now.Ticks.ToString();

        try
        {
            var response = await _paypalClient.CreateOrder(amount, currency, referenceOrderId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            var error = new { ex.GetBaseException().Message };
            return BadRequest(error);
        }
    }

    [HttpPost("/Order/capture-paypal-order")]
    [ValidateModel("Checkout")]
    public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken, [FromForm] OrderCheckoutModalDto order)
    {
        try
        {
            var response = await _paypalClient.CaptureOrder(orderID);
            var orderCreated = await _orderService.CreateOrderAsync(new OrderCheckoutDto
            {
                SameAddress = order.SameAddress,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.ReceiverAddress,
                City = order.ReceiverCity,
                Country = order.ReceiverCountry,
                AddressId = order.AddressId,
                Note = order.Note,
                PaymentMethod = PaymentMethod.PayPal,
                PaymentStatus = PaymentStatus.Paid,
                ShippingFee = order.ShippingCost,
                Discount = order.DiscountValue
            }, GetCurrentUserId());

            await _cartService.ClearCartAsync(GetCurrentUserId());

            return Ok(new { orderID = orderCreated.Id });
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
            TempData["Message"] = "Thanh toán thất bại!";
            return RedirectToAction("Fail");
        }

        var tempOrderJson = HttpContext.Session.GetString("TempOrder");
        if (string.IsNullOrEmpty(tempOrderJson))
        {
            TempData["Message"] = "Not found order information.";
            return RedirectToAction("Fail");
        }

        var order = JsonConvert.DeserializeObject<OrderCheckoutModalDto>(tempOrderJson);
        if (order == null)
        {
            TempData["Message"] = "Cannot read order information.";
            return RedirectToAction("Fail");
        }

        var orderDetail = await _orderService.CreateOrderAsync(new OrderCheckoutDto
        {
            SameAddress = order.SameAddress,
            ReceiverName = order.ReceiverName,
            ReceiverPhone = order.ReceiverPhone,
            ReceiverAddress = order.ReceiverAddress,
            City = order.ReceiverCity,
            Country = order.ReceiverCountry,
            AddressId = order.AddressId,
            Note = order.Note,
            PaymentMethod = PaymentMethod.VnPayCard,
            PaymentStatus = PaymentStatus.Paid,
            ShippingFee = order.ShippingCost,
            Discount = order.DiscountValue
        }, GetCurrentUserId());

        await _cartService.ClearCartAsync(GetCurrentUserId());
        HttpContext.Session.Remove("TempOrder");

        TempData["Message"] = "Payment successful!";
        return RedirectToAction("Success", new { orderId = orderDetail.Id });
    }

    [HttpGet("success")]
    [ValidateModel("Index")]
    public async Task<IActionResult> Success(int orderId)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();
        var orderUser = await _orderService.GetUserInfoForOrderAsync(userId, orderId);

        if (!orderUser)
        {
            return RedirectToAction("Error404", "User", new { message = "You do not have permission to access this order." });
        }

        var order = await _orderService.GetOrderDetailByIdAsync(orderId);
        var orderDetail = new OrderDetailModalDto
        {
            Id = order.Id,
            ReceiverName = order.ReceiverName,
            ReceiverPhone = order.ReceiverPhone,
            ReceiverAddress = order.ReceiverAddress,
            ReceiverCity = order.ReceiverCity,
            ReceiverCountry = order.ReceiverCountry,
            Note = order.Note,
            ShippingCost = order.ShippingCost,
            DiscountValue = order.DiscountValue,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            OrderDate = order.OrderDate,
            OrderStatus = order.Status,
            TotalAmount = order.TotalAmount,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemModalDto
            {
                ProductId = od.ProductId,
                ProductName = od.ProductName,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Subtotal = od.Subtotal,
                ProductImage = od.ProductImage,
                Size = od.Size,
            }).ToList(),

            Address = order.Address == null ? null : new AddressModalDto
            {
                AddressLine1 = order.Address.AddressLine1,
                City = order.Address.City,
                Country = order.Address.Country,
                IsDefault = order.Address.IsDefault
            }
        };

        if (orderDetail == null)
        {
            TempData["Message"] = "Not found order details.";
            return RedirectToAction("Fail");
        }

        return View("~/Order/Views/User/Success.cshtml", orderDetail);
    }

    [HttpGet("fail")]
    public IActionResult Fail(string vnp_ResponseCode)
    {
        ViewBag.ResponseCode = vnp_ResponseCode;
        ViewBag.Message = TempData["Message"];

        return View("~/Order/Views/User/Fail.cshtml");
    }

    [HttpPost("Chat")]
    [ValidateModel("Chat")]
    public async Task<IActionResult> Chat(int orderId)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();
        var userInfo = await _orderService.GetUserInfoForOrderAsync(userId, orderId);

        if (!userInfo)
        {
            return RedirectToAction("Error404", "User", new { message = "You do not have permission to access this order." });
        }

        var messages = (await _chatService.GetMessagesByOrderIdAsync(orderId))
            .Select(async x =>
            {
                string senderName;
                string senderAvatar;
                bool isAdmin = x.SenderRole?.StartsWith("Admin") == true;

                if (isAdmin)
                {
                    var admin = await _chatService.GetCurrentAdminAsync(x.SenderId);
                    senderName = admin?.UserName ?? "Admin";
                    senderAvatar = admin?.AvatarUrl ?? "~/assets/images/admin-default.jpg";
                }
                else
                {
                    var user = await _userService.GetUserByIdAsync(x.SenderId);
                    senderName = user?.UserName ?? "User";
                    senderAvatar = user?.AvatarUrl ?? "~/assets/images/user-default.png";
                }

                return new MessageDto
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    Content = x.Content,
                    SentAt = x.SentAt,
                    OrderId = x.OrderId,
                    SenderName = senderName,
                    SenderAvatar = senderAvatar,
                    SenderRole = x.SenderRole
                };
            })
            .Select(t => t.Result)
            .OrderBy(m => m.SentAt)
            .ToList();

        var chatDto = new ChatModalDto
        {
            OrderId = orderId,
            Message = messages,
            CurrentUserAvatar = (await _userService.GetUserByIdAsync(userId))?.AvatarUrl ?? "~/assets/images/user-default.png"
        };

        return View("~/Order/Views/User/Chat/index.cshtml", chatDto);
    }
}