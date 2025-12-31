using System.ComponentModel;

namespace ShoesShop.Domain.Users.Enums;
public enum UserAccountRole
{
    [Description("Admin")]
    Admin = 10,

    [Description("Customer")]
    Customer = 20
}