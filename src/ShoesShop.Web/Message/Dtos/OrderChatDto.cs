using ShoesShop.Domain.Orders.Dtos;
using ShoesShop.Domain.Shares.Messages.Dtos;
using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Web.Message.Dtos;

public class OrderChatDto
{
    public OrderDto Order { get; set; }
    public List<MessageDto> Messages { get; set; } = [];
    public MessageDto LastMessage { get; set; }
    public UserDto User { get; set; }
}
