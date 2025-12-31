using ShoesShop.Domain.Orders.Dtos;
using ShoesShop.Domain.Shares.Messages.Dtos;
using ShoesShop.Domain.Shares.Messages.Entity;
using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Domain.Shares.Messages.Services;

public interface IChatService
{
    Task<Message> SaveMessage(int senderId, int receiverId, string content, int orderId, string senderRole, string senderName, string senderAvatar);
    Task<List<Message>> GetMessagesByOrderIdAsync(int orderId);

    // Thêm cho admin
    Task<List<OrderDto>> GetAllOrdersFromMessagesAsync();
    Task<List<MessageDto>> GetAllMessagesAsync();
    Task<OrderDto> GetOrder(int orderId);
    Task<UserDto> GetCurrentAdminAsync(int adminId);
}