using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Orders.Dtos;
using ShoesShop.Domain.Orders.Dtos.Commands;
using ShoesShop.Domain.Orders.Entities;
using ShoesShop.Domain.Shares.Image.Entities;
using ShoesShop.Domain.Shares.Image.Enums;
using ShoesShop.Domain.Shares.Messages.Dtos;
using ShoesShop.Domain.Shares.Messages.Entity;
using ShoesShop.Domain.Shares.Messages.Services;
using ShoesShop.Domain.Users.Dtos;
using ShoesShop.Domain.Users.Entities;

namespace ShoesShop.Domain.Services.Modules.Messages;

public class ChatService : IChatService
{
    private readonly IGenericRepository<Message, int> _messageRepository;
    private readonly IGenericRepository<Order, int> _orderRepository;
    private readonly IGenericRepository<Image, int> _imageRepository;

    private readonly IGenericRepository<ImageUser, int> _imageUserRepository;
    private readonly IGenericRepository<ImageProduct, int> _imageProductRepository;


    private readonly IGenericRepository<User, int> _userRepository;

    public ChatService(
        IGenericRepository<Message, int> messageRepository,
        IGenericRepository<User, int> userRepository,
        IGenericRepository<Order, int> orderRepository,
        IGenericRepository<Image, int> imageRepository,
        IGenericRepository<ImageUser, int> imageUserRepository,
        IGenericRepository<ImageProduct, int> imageProductRepository)
    {
        _messageRepository = messageRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _imageRepository = imageRepository;
        _imageUserRepository = imageUserRepository;
        _imageProductRepository = imageProductRepository;
    }

    public async Task<Message> SaveMessage(int senderId, int receiverId, string content,int orderId,
        string? senderRole, string? senderName, string? senderAvatar)
    {
        senderRole ??= "User";

        string defaultAvatar;
        string defaultName;

        var sender = await _userRepository.GetByIdAsync(senderId)
            ?? throw new InvalidOperationException($"User {senderId} không tồn tại");

        if (senderRole.StartsWith("Admin", StringComparison.OrdinalIgnoreCase))
        {
            defaultName = "Admin";
            defaultAvatar = "/assets/images/admin-default.jpg";
        }
        else
        {
            defaultName = "User";
            defaultAvatar = "/images/default.png";
        }

        senderName ??= sender.UserName ?? defaultName;

        if (string.IsNullOrWhiteSpace(senderAvatar))
        {
            var avatarImage = await _imageUserRepository
                .GetAsync(i =>
                    i.OwnerType == OwnerType.User &&
                    i.OwnerId == senderId);

            senderAvatar = avatarImage?.Url ?? defaultAvatar;
        }

        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new InvalidOperationException($"Order {orderId} không tồn tại");

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
                .Include(o => o.User)
                    .ThenInclude(u => u.Addresses)
        );

        var productIds = orders
            .SelectMany(o => o.OrderDetails)
            .Select(od => od.Product.Id)
            .Distinct()
            .ToList();

        var userIds = orders
            .Select(o => o.User.Id)
            .Distinct()
            .ToList();

        var imagesUser = new List<ImageUser>();
        imagesUser.AddRange(await _imageUserRepository.GetAllAsync(x => userIds.Contains(x.OwnerId)));
        var imagesProduct = new List<ImageProduct>();
        imagesProduct.AddRange(await _imageProductRepository.GetAllAsync(x => productIds.Contains(x.OwnerId)));

        return orders.Select(order =>
        {
            decimal total = order.OrderDetails.Sum(od => od.Subtotal);
            decimal discount = order.Discount ?? 0;
            decimal ship = order.ShippingFee ?? 0;

            var userAvatar = imagesUser
                .FirstOrDefault(i =>
                    i.OwnerType == OwnerType.User &&
                    i.OwnerId == order.User.Id)
                ?.Url ?? "/images/default.png";

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

                OrderDetails = order.OrderDetails.Select(d =>
                {
                    var productImage = imagesProduct
                        .FirstOrDefault(i =>
                            i.OwnerType == OwnerType.Product &&
                            i.OwnerId == d.Product.Id)
                        ?.Url;

                    return new OrderDetailItemDto
                    {
                        ProductId = d.Product.Id,
                        ProductName = d.Product.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Subtotal = d.Subtotal,
                        Size = d.Size,
                        ProductImage = productImage
                    };
                }).ToList(),

                User = new UserDto
                {
                    ID = order.User.Id,
                    UserName = order.User.UserName,
                    Email = order.User.Email,
                    Phone = order.User.Phone,
                    DateOfBirth = order.User.DateOfBirth,
                    Gender = order.User.Gender,
                    AvatarUrl = userAvatar,
                    Addresses = order.User.Addresses.Select(a => new AddressDto
                    {
                        AddressLine1 = a.AddressLine1,
                        City = a.City,
                        Country = a.Country,
                        IsDefault = a.IsDefault
                    }).ToList()
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
        var admin = await _userRepository.GetByIdAsync(adminId)
            ?? throw new Exception("Admin not found");

        var avatar = await _imageUserRepository.GetAsync(i =>
            i.OwnerType == OwnerType.User &&
            i.OwnerId == admin.Id
        );

        return new UserDto
        {
            ID = admin.Id,
            UserName = admin.UserName,
            Email = admin.Email,
            Phone = admin.Phone,
            DateOfBirth = admin.DateOfBirth,
            Gender = admin.Gender,
            AvatarUrl = avatar?.Url ?? "/assets/images/admin-default.jpg"
        };
    }

    public async Task<OrderDto> GetOrder(int orderId)
    {
        var result = await _orderRepository.GetByIdAsync(orderId);
        return new OrderDto { Id = orderId , User = new UserDto { ID =  result.User.Id} };
    }

}
