using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum OrderStatus
{
    [Description("Pending")]
    Pending = 0,

    [Description("Processing")]
    Processing = 1,

    [Description("Completed")]
    Completed = 2,

    [Description("Cancelled")]
    Cancelled = 3
}