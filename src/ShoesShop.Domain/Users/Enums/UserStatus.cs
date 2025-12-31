using System.ComponentModel;

namespace ShoesShop.Domain.Users.Enums;

public enum UserStatus
{
    [Description("Pending")]
    InConfirm = 10,

    [Description("Active")]
    Active = 20,

    [Description("InActive")]
    InActive = 30,
    
    [Description("Banned")]
    Banned = 40
}