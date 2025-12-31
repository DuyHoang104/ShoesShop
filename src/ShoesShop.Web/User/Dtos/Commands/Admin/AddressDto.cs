namespace ShoesShop.Web.User.Dtos.Commands.Admin;

public class AddressDto
{
    public string AddressLine1 { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }

    public bool IsDefault { get; set; } = false;
}