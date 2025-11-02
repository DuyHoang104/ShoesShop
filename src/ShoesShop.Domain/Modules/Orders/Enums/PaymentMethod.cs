using System.ComponentModel;

namespace ShoesShop.Domain.Modules.Orders.Enums;

public enum PaymentMethod
{
    [Description("Cash on delivery")]
    Cash = 10,

    [Description("QR Code")]
    QRCode = 20,

    [Description("PayPal")]
    PayPal = 30,

    [Description("Card")]
    Card = 40
}