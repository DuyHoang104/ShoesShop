using System.ComponentModel;

namespace ShoesShop.Domain.Modules.User.Users.Enums
{
    public enum UserGender
    {
        [Description("Male")]
        Male = 10,

        [Description("Female")]
        Female = 20,

        [Description("Others")]
        Others = 30
    }
}