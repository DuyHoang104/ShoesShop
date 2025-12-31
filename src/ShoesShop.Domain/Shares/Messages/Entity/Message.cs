using ShoesShop.Domain.Commons.Entities;
using ShoesShop.Domain.Orders.Entities;

namespace ShoesShop.Domain.Shares.Messages.Entity;

public partial class Message : BaseEntity<int>
{
    private int _senderId;
    public int SenderId
    {
        get => _senderId;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(SenderId));
            _senderId = value;
        }
    }

    private int _receiverId;
    public int ReceiverId
    {
        get => _receiverId;
        set
        {
            _receiverId = value;
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(Content));
            if (value.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(Content));
            _content = value;
        }
    }

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    private string _senderRole = string.Empty;
    public string SenderRole
    {
        get => _senderRole;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(SenderRole));
            _senderRole = value;
        }
    }

    private string _senderName = string.Empty;
    public string SenderName
    {
        get => _senderName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(SenderName));
            _senderName = value;
        }
    }

    private string _senderAvatar = string.Empty;
    public string SenderAvatar
    {
        get => _senderAvatar;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(SenderAvatar));
            _senderAvatar = value;
        }
    }
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public Message(int senderId, int receiverId, string content, string senderName, string senderAvatar, string senderRole, Order order)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        IsRead = false;
        SentAt = DateTime.Now;
        SenderName = senderName;
        SenderAvatar = senderAvatar;
        SenderRole = senderRole;
        Order = order;
        OrderId = order.Id;
    }

    public Message() { }
}
