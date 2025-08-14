using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Users.Enums
{
    public enum UserGender
    {
        [Description("Male")]
        Male = 1,

        [Description("Female")]
        Female = 2,

        [Description("Others")]
        Others = 3
    }
}