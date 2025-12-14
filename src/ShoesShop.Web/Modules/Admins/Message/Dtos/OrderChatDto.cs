using ShoesShop.Domain.Modules.Shares.Messages.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos;

namespace ShoesShop.Web.Modules.Admins.Message.Dtos;

public class OrderChatDto
{
    public OrderDto Order { get; set; }
    public List<MessageDto> Messages { get; set; } = [];
    public MessageDto LastMessage { get; set; }
    public UserDto User { get; set; }
}
