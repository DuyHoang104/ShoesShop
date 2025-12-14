using ShoesShop.Domain.Modules.Messages.Entity;
using ShoesShop.Domain.Modules.Shares.Messages.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Domain.Modules.Shares.Messages.Services;
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