namespace ShoesShop.Web.Modules.Admins.Orders.Dtos
{
    public class AddressModalAdminDto
    {
        public string AddressLine1 { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}