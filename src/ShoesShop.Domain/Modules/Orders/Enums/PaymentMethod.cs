using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum PaymentMethod
{
    [Description("Cash on delivery")]
    Cash = 0,

    [Description("QR Code")]
    QRCode = 1,

    [Description("PayPal")]
    PayPal = 2,

    [Description("Card")]
    Card = 3
}