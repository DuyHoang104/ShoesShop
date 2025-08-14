using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Users.Enums
{
    public enum UserAccountRole
    {
        [Description("Admin")]
        Admin = 0,

        [Description("Manager")]
        Manager = 1,

        [Description("Employee")]
        Employee = 2
    }
}