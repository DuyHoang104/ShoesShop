namespace ShoesShop.Web.Modules.Admins.Users.Dtos;

public class AddressAdminDto
{
    public string AddressLine1 { get; set; } = string.Empty;

    public string? City { get; set; }

    public string? Country { get; set; }

    public bool IsDefault { get; set; } = false;
}