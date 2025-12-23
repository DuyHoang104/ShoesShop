using ShoesShop.Domain.Modules.Shares.Image.Enums;
using ShoesShop.Domain.Modules.User.Commons.Entities;

namespace ShoesShop.Domain.Modules.Shares.Image.Entities;

public abstract class Image : BaseEntity<int>
{
    private string _url = string.Empty;

    public string Url
    {
        get => _url;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Image URL cannot be empty or whitespace.", nameof(Url));

            if (value.Length > 2048)
                throw new ArgumentOutOfRangeException(nameof(Url), "Image URL cannot exceed 2048 characters.");

            _url = value;
        }
    }

    public int OwnerId { get; set; }

    private OwnerType _ownerType;
    public OwnerType OwnerType
    {
        get => _ownerType;
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException("Owner Type cannot be empty or whitespace.", nameof(OwnerType));

            _ownerType = value;
        }
    }

    public string PublicId { get; set; } = null!;
}