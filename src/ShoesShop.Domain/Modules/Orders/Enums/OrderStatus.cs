using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum OrderStatus
{
    [Description("Pending")]
    Pending = 10,

    [Description("Processing")]
    Processing = 20,

    [Description("Completed")]
    Completed = 30,

    [Description("Cancelled")]
    Cancelled = 40
}