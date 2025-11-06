namespace ShoesShop.Web.Modules.Order.Dtos.Commands
{
    public class AddressModalDto
    {
        public string AddressLine1 { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}