using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Users.Enums
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