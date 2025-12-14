using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.Crosscutting.Utilities.Attribute;
using ShoesShop.Domain.Modules.Shares.Messages.Dtos;
using ShoesShop.Domain.Modules.Shares.Messages.Services;
using ShoesShop.Domain.Modules.User.Orders.Services;
using ShoesShop.Web.Modules.Admins.Message.Dtos;

namespace ShoesShop.Web.Modules.Admins.Message.Controllers;

[Authorize(AuthenticationSchemes = "AdminScheme", Roles = "Admin")]
[Route("Admin/Messages")]
public class MessageAdminController : Controller
{
    private readonly IChatService _chatService;
    private readonly IOrderService _orderService;

    public MessageAdminController(IChatService chatService, IOrderService orderService)
    {
        _chatService = chatService;
        _orderService = orderService;
    }

    private int GetCurrentAdminId()
    {
        return int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "adminId")?.Value, out var adminId)
            ? adminId
            : 0;
    }

    private async Task<List<OrderChatDto>> BuildOrderChatList()
    {
        var allMessages = await _chatService.GetAllMessagesAsync();
        var ListOrders = await _chatService.GetAllOrdersFromMessagesAsync();

        return ListOrders.Select(order =>
        {
            var msgs = allMessages
                .Where(m => m.OrderId.HasValue && m.OrderId.Value == order.Id)
                .OrderBy(m => m.SentAt)
                .ToList();

            return new OrderChatDto
            {
                Order = order,
                Messages = msgs,
                LastMessage = msgs.LastOrDefault(),
                User = order.User
            };
        })
        .OrderByDescending(c => c.LastMessage?.SentAt ?? DateTime.MinValue)
        .ToList();
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var orderChats = await BuildOrderChatList();
        return View("~/Modules/Admins/Message/Views/index.cshtml", orderChats);
    }

    [HttpPost("Chat")]
    [ValidateModel("index")]
    public async Task<IActionResult> Chat(int orderId)
    {
        var adminId = GetCurrentAdminId();
        if (adminId == 0) return Unauthorized();

        var orderChats = await BuildOrderChatList();
        var selectedChat = orderChats.FirstOrDefault(x => x.Order != null && x.Order.Id == orderId);

        if (selectedChat == null)
        {
            ViewData["Messages"] = new List<MessageDto>();
            ViewBag.SelectedOrderId = null;
            ViewBag.UserId = 0;
            return View("~/Modules/Admins/Message/Views/index.cshtml", orderChats);
        }

        ViewData["Messages"] = selectedChat.Messages;
        ViewData["OrderInfo"] = new
        {
            orderId = selectedChat.Order.Id,
            OrderDate = selectedChat.Order.OrderDate,
            ReceiverName = selectedChat.Order.ReceiverName,
            Status = selectedChat.Order.Status,
            PaymentMethod = selectedChat.Order.PaymentMethod,
            TotalAmount = selectedChat.Order.TotalAmount,
            PaymentStatus = selectedChat.Order.PaymentStatus,
        };

        ViewBag.SelectedOrderId = selectedChat.Order.Id;
        ViewBag.UserId = selectedChat.User.ID;

        var currentAdmin = await _chatService.GetCurrentAdminAsync(adminId);
        ViewBag.AdminId = currentAdmin.ID;
        ViewBag.AdminName = currentAdmin.UserName;
        ViewBag.AdminAvatarUrl = currentAdmin.AvatarUrl ?? "/assets/Admin/images/users/admin-avatar.png";

        return View("~/Modules/Admins/Message/Views/index.cshtml", orderChats);
    }
}