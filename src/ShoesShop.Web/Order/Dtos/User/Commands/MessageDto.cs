namespace ShoesShop.Web.Modules.Users.Order.Dtos.Commands;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public int? OrderId { get; set; }
    public string SenderName { get; set; }
    public string SenderAvatar { get; set; }
    public string SenderRole { get; set; }
}