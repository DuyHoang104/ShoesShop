using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.Messages.Entity;
using ShoesShop.Domain.Modules.Shares.Messages.Dtos;
using ShoesShop.Domain.Modules.Shares.Messages.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Domain.Services.Modules.Messages;
public class ChatService : IChatService
{
    private readonly IGenericRepository<Message, int> _messageRepository;
    private readonly IGenericRepository<Order, int> _orderRepository;

    private readonly IGenericRepository<Domain.Modules.User.Users.Entities.User, int> _userRepository;

    public ChatService(
        IGenericRepository<Message, int> messageRepository,
        IGenericRepository<Domain.Modules.User.Users.Entities.User, int> userRepository,
        IGenericRepository<Order, int> orderRepository)
    {
        _messageRepository = messageRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<Message> SaveMessage(int senderId, int receiverId, string content, int orderId,
        string senderRole, string senderName, string senderAvatar)
    {
        if (string.IsNullOrWhiteSpace(senderRole))
            senderRole = "User";

        if (senderRole.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
        {
            var admin = await _userRepository.GetByIdAsync(senderId);
            senderName ??= admin?.UserName ?? "Admin";
            senderAvatar ??= admin?.AvatarUrl ?? "/assets/images/admin-default.jpg";
        }
        else
        {
            var user = await _userRepository.GetByIdAsync(senderId);
            senderName ??= user?.UserName ?? "User";
            senderAvatar ??= user?.AvatarUrl ?? "/images/default.png";
        }

        var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new InvalidOperationException($"Order {orderId} không tồn tại");

        var msg = new Message(
            senderId,
            receiverId,
            content,
            senderName,
            senderAvatar,
            senderRole,
            order
        );

        await _messageRepository.InsertAsync(msg);
        await _messageRepository.SaveChangesAsync();

        return msg;
    }

    public async Task<List<Message>> GetMessagesByOrderIdAsync(int orderId)
    {
        var messages = await _messageRepository.GetAllAsync(
            predicate: m => m.OrderId == orderId
        );

        return messages
            .OrderBy(m => m.SentAt)
            .ToList();
    }

    public async Task<List<OrderDto>> GetAllOrdersFromMessagesAsync()
    {
        var messages = await _messageRepository.GetAllAsync(
            predicate: m => m.OrderId != null
        );

        var orderIds = messages
        .Select(m => m.OrderId)
        .Distinct()
        .ToList();

        if (orderIds.Count == 0)
            return [];

        var orders = await _orderRepository.GetAllAsync(
            predicate: o => orderIds.Contains(o.Id),
            include: q => q
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Images)
                .Include(o => o.User)
                .ThenInclude(u => u.Addresses)
        );

        return orders.Select(order =>
        {
            decimal total = order.OrderDetails.Sum(od => od.Subtotal);
            decimal discount = order.Discount ?? 0;
            decimal ship = order.ShippingFee ?? 0;

            return new OrderDto
            {
                Id = order.Id,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingFee = order.ShippingFee,
                Discount = order.Discount,
                ReceiverName = order.ReceiverName,
                TotalAmount = (total * (1 - discount)) + ship,

                OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
                {
                    ProductID = d.Product.Id,
                    ProductName = d.Product?.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal,
                    Size = d.Size,
                    ProductImage = d.Product?.Images?.FirstOrDefault()?.Url
                }).ToList(),

                User = new UserDto
                {
                    ID = order.User.Id,
                    UserName = order.User.UserName,
                    Email = order.User.Email,
                    Phone = order.User.Phone,
                    DateOfBirth = order.User.DateOfBirth,
                    Gender = order.User.Gender,
                    AvatarUrl = order.User.AvatarUrl,
                    Addresses = order.User.Addresses?.Select(a => new AddressDto
                    {
                        AddressLine1 = a.AddressLine1,
                        City = a.City,
                        Country = a.Country,
                        IsDefault = a.IsDefault
                    }).ToList() ?? []
                }
            };
        }).ToList();
    }

    // GET ALL MESSAGES
    public async Task<List<MessageDto>> GetAllMessagesAsync()
    {
        var messages = await _messageRepository.GetAllAsync();
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            Content = m.Content,
            SentAt = m.SentAt,
            OrderId = m.OrderId,
            SenderRole = m.SenderRole,
            SenderName = m.SenderName,
            SenderAvatar = m.SenderAvatar,
            IsRead = false,
        }).ToList();
    }
    
    public async Task<UserDto> GetCurrentAdminAsync(int adminId)
    {
        var adminEntity = await _userRepository.GetByIdAsync(adminId) ?? throw new Exception("Admin not found");

        return new UserDto
        {
            ID = adminEntity.Id,
            UserName = adminEntity.UserName,
            Email = adminEntity.Email,
            Phone = adminEntity.Phone,
            DateOfBirth = adminEntity.DateOfBirth,
            Gender = adminEntity.Gender,
            AvatarUrl = adminEntity.AvatarUrl
        };
    }

    public async Task<OrderDto> GetOrder(int orderId)
    {
        var result = await _orderRepository.GetByIdAsync(orderId);
        return new OrderDto { Id = orderId , User = new UserDto { ID =  result.User.Id} };
    }

}
