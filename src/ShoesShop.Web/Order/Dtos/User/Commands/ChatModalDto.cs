namespace ShoesShop.Web.Modules.Users.Order.Dtos.Commands;

public class ChatModalDto
{
    public int OrderId { get; set; }
    public List<MessageDto> Message { get; set; } = [];
    public string CurrentUserAvatar { get; set; } = string.Empty;
}