using System.ComponentModel;

namespace ShoesShop.Domain.Shares.Review.Enums;

public enum ReviewStatus
{
    [Description("Active")]
    Active = 10,

    [Description("Hidden")]
    Hidden = 20
}