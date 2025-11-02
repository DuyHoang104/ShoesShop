using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum PaymentStatus
{
    [Description("Unpaid")]
    Unpaid = 10,

    [Description("Paid")]
    Paid = 20,

    [Description("Failed")]
    Failed = 30
}