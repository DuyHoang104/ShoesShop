using ShoesShop.Domain.Commons.Entities;
using ShoesShop.Domain.Shares.Image.Entities;

namespace ShoesShop.Domain.Shares.Review.Entity;

public class Review : EntityAuditLog<int>
{
    private string _comment = string.Empty;
    public string Comment
    {
        get => _comment;
        set
        {
            if (value.Length > 1000)
                throw new ArgumentOutOfRangeException(nameof(Comment));
            _comment = value;
        }
    }

    private int _rating;
    public int Rating
    {
        get => _rating;
        set
        {
            if (value < 1 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(Rating));
            _rating = value;
        }
    }

    public object? Metadata { get; set; }

    public int? ParentId { get; private set; }
    public Review? Parent { get; private set; }

    private readonly List<Review> _children = new();
    public IReadOnlyCollection<Review> Children => _children;

    public void AddChild(Review child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (child.ParentId != null)
            throw new InvalidOperationException("Review already has a parent.");

        child.ParentId = Id;
        child.Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(Review child)
    {
        ArgumentNullException.ThrowIfNull(child);

        if (_children.Remove(child))
        {
            child.ParentId = null;
            child.Parent = null;
        }
    }

    private readonly HashSet<ImageReview> _images = new();
    public IReadOnlyCollection<ImageReview> Images => _images;

    public void AddImage(string url, string publicId)
    {
        if (_images.Any(i => i.PublicId == publicId))
            throw new InvalidOperationException("Image already exists.");

        _images.Add(new ImageReview(url, publicId));
    }

    public void RemoveImage(string publicId)
    {
        var image = _images.FirstOrDefault(i => i.PublicId == publicId)
            ?? throw new InvalidOperationException("Image not found.");

        _images.Remove(image);
    }

    public Review() { }

    public Review(int rating, string comment, object? metadata = null, int? parentId = null)
    {
        Rating = rating;
        Comment = comment;
        Metadata = metadata;
        ParentId = parentId;
    }
}
