using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Shares.Enums;
using ShoesShop.Domain.Modules.Users.Entities;

namespace ShoesShop.Domain.Modules.Shares.Entities;
public class ObjectChangeLog : BaseEntity<int>
{
    private EntityName _entityName;
    public EntityName EntityName
    {
        get => _entityName;
        set
        {
            if (!Enum.IsDefined(typeof(EntityName), value))
            {
                throw new ArgumentException("Invalid entity name.", nameof(value));
            }

            _entityName = value;
        }
    }

    private string _propertyName = string.Empty;
    public string PropertyName
    {
        get => _propertyName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(value));
            }

            _propertyName = value;
        }
    }

    private int _keyValue;
    public int KeyValue
    {
        get => _keyValue;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Key value must be greater than 0.", nameof(value));
            }

            _keyValue = value;
        }
    }

    private string? _oldValue;
    public string? OldValue
    {
        get => _oldValue;
        set
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Old value cannot be null or empty.", nameof(value));
                }

                _oldValue = value;
            }
        }
    }

    private string? _newValue;
    public string? NewValue
    {
        get => _newValue;
        set
        {
            if (value != null)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("New value cannot be null or empty.", nameof(value));
                }

                _newValue = value;
            }
        }
    }

    private DateTime _changeDate = DateTime.UtcNow;
    public DateTime ChangeDate
    {
        get => _changeDate;
        set
        {
            if (value < DateTime.UtcNow)
            {
                throw new ArgumentException("Change date cannot be in the past.", nameof(value));
            }
            _changeDate = value;
        }
    }

    private int _userId;
    public int UserId
    {
        get => _userId;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("User ID must be greater than 0.", nameof(value));
            }

            _userId = value;
        }
    }

    private User _user = null!;
    public User User
    {
        get => _user;
        set
        {
            if (value == null)      
            {
                throw new ArgumentNullException(nameof(User), "User cannot be null.");
            }

            _user = value;
            _userId = value.Id;
        }
    }

    public ObjectChangeLog(EntityName entityName, string propertyName, int keyValue, string? oldValue, string? newValue, DateTime changeDate, User user)
    {
        EntityName = entityName;
        PropertyName = propertyName;
        KeyValue = keyValue;
        OldValue = oldValue;
        NewValue = newValue;
        ChangeDate = changeDate;
        User = user;
    }

    public ObjectChangeLog() { }
}