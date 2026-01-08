using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShoesShop.Domain.Orders.Services;
using ShoesShop.Domain.Shares.Messages.Dtos;
using ShoesShop.Domain.Shares.Messages.Services;
using ShoesShop.Domain.Users.Enums;
using ShoesShop.Domain.Users.Services;

namespace ShoesShop.Domain.Shares.Messages.Hubs;

[Authorize(AuthenticationSchemes = "UserScheme,AdminScheme")]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public ChatHub(IChatService chatService, IUserService userService, IOrderService orderService)
    {
        _chatService = chatService;
        _userService = userService;
        _orderService = orderService;
    }

    public async Task JoinOrderGroup(int orderId)
    {
        if (orderId <= 0) return;

        var callerInfo = await GetCallerInfoAsync();
        if (callerInfo == null) return;

        var (senderId, role) = callerInfo.Value;

        if (role == UserAccountRole.Customer)
        {
            bool isOwner = await _orderService.CheckUserInfoForOrderAsync(senderId, orderId);
            if (!isOwner)
            {
                Console.WriteLine($"Access Denied: User {senderId} tried to join not owned order {orderId}");
                throw new HubException("ACCESS_DENIED_ORDER_NOT_OWNED"); 
            }
        }

        string connectionId = Context.ConnectionId;
        string groupName = $"OrderGroup_{orderId}";

        await Groups.AddToGroupAsync(connectionId, groupName);

        Console.WriteLine($"Join SUCCESS: {role}={senderId} → order {orderId}");
    }

    public async Task SendMessage(int receiverId, string content, int orderId, string tempId)
    {
        if (string.IsNullOrWhiteSpace(content) || orderId <= 0)
        {
            Console.WriteLine("Error: content empty or orderId invalid");
            return;
        }
        
        string connectionId = Context.ConnectionId;
        var callerInfo = await GetCallerInfoAsync();
        if (callerInfo == null) return;

        var (senderId, senderRole) = callerInfo.Value;

        var senderUser = await _userService.GetUserByIdAsync(senderId);
        if (senderUser == null) return;

        string senderName = senderUser.UserName ?? (senderRole == UserAccountRole.Admin ? "Admin" : "User");
        string defaultAvatar = senderRole == UserAccountRole.Admin ? "/assets/images/admin-default.jpg" : "/images/default.png";
        string senderAvatar = !string.IsNullOrEmpty(senderUser.AvatarUrl) ? senderUser.AvatarUrl : defaultAvatar;

        int finalReceiverId = (senderRole == UserAccountRole.Customer && receiverId == 0) ? -1 : receiverId;

        var message = await _chatService.SaveMessage(
            senderId, finalReceiverId, content, orderId,
            senderRole.ToString(), senderName, senderAvatar
        );

        var dto = new MessageDto
        {
            Id = message.Id,
            SenderId = senderId,
            ReceiverId = finalReceiverId,
            Content = content,
            OrderId = orderId,
            SenderRole = senderRole.ToString(),
            SenderName = senderName,
            SenderAvatar = senderAvatar,
            SentAt = DateTime.Now,
            TempId = tempId
        };

        string groupName = $"OrderGroup_{orderId}";
        Console.WriteLine($"Broadcast message → Order {orderId} from {senderName}");

        await Clients.Group(groupName).SendAsync("ReceiveMessage", dto);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<(int userId, UserAccountRole role)?> GetCallerInfoAsync()
    {
        string? admin = Context.User?.FindFirst("adminId")?.Value;
        string? user = Context.User?.FindFirst("userId")?.Value;

        if (!string.IsNullOrEmpty(admin) && int.TryParse(admin, out int senderId))
        {
            return (senderId, UserAccountRole.Admin);
        }

        if (!string.IsNullOrEmpty(user) && int.TryParse(user, out senderId))
        {
            return (senderId, UserAccountRole.Customer);
        }

        return null;
    }
}