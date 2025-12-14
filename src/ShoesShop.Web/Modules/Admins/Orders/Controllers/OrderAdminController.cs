using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.User.Orders.Enums;
using ShoesShop.Domain.Modules.User.Orders.Services;
using ShoesShop.Domain.Modules.User.Users.Dtos;
using ShoesShop.Web.Modules.Admins.Orders.Dtos;

namespace ShoesShop.Web.Modules.Admins.Orders.Controllers;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin/Orders")]
public class OrderAdminController : Controller
{
    private readonly IOrderService _orderService;

    public OrderAdminController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetCurrentUserId()
    {
        return int.TryParse(User.FindFirst("userId")?.Value, out var userId) ? userId : 0;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrderAsync(null);
        var ordermodalDto = new List<OrderAdminModalDto>(
            orders.Select(o => new OrderAdminModalDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemAdminModalDto
                {
                    ProductID = od.ProductID,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal,
                    Size = od.Size,
                    ProductImage = od.ProductImage
                }).ToList()
            })
        );

        return View("~/Modules/Admins/Orders/Views/apps-ecommerce-orders.cshtml", ordermodalDto);
    }

    [HttpGet("Details/{id}")]
    [ValidateModel("Admin/Orders")]
    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderDetailByIdAsync(id);
        var orderDetail = new OrderDetailAdminModalDto
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
            Status = order.Status,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemAdminModalDto
            {
                ProductID = od.ProductID,
                ProductName = od.ProductName,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Subtotal = od.Subtotal,
                ProductImage = od.ProductImage
            }).ToList(),

            Address = order.Address == null ? null : new AddressModalAdminDto
            {
                AddressLine1 = order.Address.AddressLine1,
                City = order.Address.City,
                Country = order.Address.Country,
                IsDefault = order.Address.IsDefault
            }
        };

        if (orderDetail == null)
        {
            return RedirectToAction("Error404", "Admin", new { message = "Only admin can log in here." });

        }

        return View("~/Modules/Admins/Orders/Views/apps-ecommerce-orders-details.cshtml", orderDetail);
    }

    [HttpPost("UpdateStatus/{id}")]
    [ValidateModel("Admin/Orders")]
    public async Task<IActionResult> UpdateStatus(int id, [FromForm] OrderStatus newStatus)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, newStatus);
        if (result)
        {
            return Json(new { success = true, message = "Order status updated successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to update order status." });
        }
    }

    [HttpGet("ExportInvoice/{id}")]
    [ValidateModel("Admin/Orders")]
    public async Task<IActionResult> ExportInvoice(int id)
    {
        var order = await _orderService.GetOrderDetailByIdAsync(id);
        var orderDetail = new OrderDetailAdminModalDto
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
            Status = order.Status,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderDetails = order.OrderDetails.Select(od => new OrderDetailItemAdminModalDto
            {
                ProductID = od.ProductID,
                ProductName = od.ProductName,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Subtotal = od.Subtotal,
                ProductImage = od.ProductImage
            }).ToList(),

            Address = order.Address == null ? null : new AddressModalAdminDto
            {
                AddressLine1 = order.Address.AddressLine1,
                City = order.Address.City,
                Country = order.Address.Country,
                IsDefault = order.Address.IsDefault
            },

            User =  new UserDto
            {
                UserName = order.User.UserName,
                Email = order.User.Email,
                Phone = order.User.Phone,
                Addresses = order.User.Addresses
            }
        };

        if (orderDetail == null)
        {
            return RedirectToAction("Error404", "Admin", new { message = "Only admin can log in here." });
        }
        
        return View("~/Modules/Admins/Orders/Views/pages-invoice.cshtml", orderDetail);
    }
}