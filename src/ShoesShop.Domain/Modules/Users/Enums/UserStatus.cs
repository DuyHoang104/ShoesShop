using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Users.Enums
{
    public enum UserStatus
    {
        [Description("Pending")]
        InConfirm = 10,

        [Description("Active")]
        Active = 20,

        [Description("Inactive")]
        Inactive = 30
    }
}