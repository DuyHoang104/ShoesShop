using System.ComponentModel;

namespace ShoesShop.Domain.Shares.ObjectChangeLog.Enums;

public enum EntityName
{
    [Description("User Status")]
    UserStatus = 10,

    [Description("Product")]
    Product = 20,

    [Description("Category")]
    Category = 30
}