using ShoesShop.Domain.Modules.User.Categories.Enums;
using ShoesShop.Domain.Modules.User.Commons.Entities;
using ShoesShop.Domain.Modules.User.Products.Entities;

namespace ShoesShop.Domain.Modules.User.Categories.Entities;

public class Category : EntityAuditLog<int>
{
    private string _name = string.Empty;
    public string Name 
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be empty or whitespace.", nameof(Name));
            }
            if (value.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Name), "Name cannot exceed 100 characters.");
            }

            _name = value;
        }
    }

    private string _description = string.Empty;
    public string Description 
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Description cannot be empty or whitespace.", nameof(Description));
            }
            if (value.Length > 200)
            {
                throw new ArgumentOutOfRangeException(nameof(Description), "Description cannot exceed 500 characters.");
            }

            _description = value;
        }
    }

    private CategoryStatus _status;
    public CategoryStatus Status 
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(typeof(CategoryStatus), value))
            {
                throw new ArgumentOutOfRangeException(nameof(Status), "Invalid status value.");
            }

            _status = value;
        }
    }
    
    private readonly HashSet<ProductCategory> _productCategories = [];
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories;

    public Category(string name, string description, CategoryStatus status)
    {
        Name = name;
        Description = description;
        Status = status;
    }
    
    public Category(string name, string description, CategoryStatus status, int adminId) : this(name, description, status)
    {
        CreateBy = adminId;
        CreateTimeStamp = DateTime.UtcNow;

        LastActionBy = adminId;
        LastAction = Domain.Modules.User.Commons.Enums.LastAction.Create;
        LastActionTimeStamp = DateTime.UtcNow;
    }

    public Category() { }
}