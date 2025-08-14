using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum PaymentStatus
{
    [Description("Unpaid")]
    Unpaid = 0,

    [Description("Paid")]
    Paid = 1,

    [Description("Failed")]
    Failed = 2
}