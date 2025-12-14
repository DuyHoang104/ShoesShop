namespace ShoesShop.Domain.Modules.Shares.Messages.Dtos;
public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string SenderRole { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public int? OrderId { get; set; }
    public string SenderName { get; set; }
    public string SenderAvatar { get; set; }
    public bool IsAdmin => SenderRole.StartsWith("Admin");
    public bool IsRead { get; set; } = false;

    public string TempId { get; set; } = Guid.NewGuid().ToString();
}