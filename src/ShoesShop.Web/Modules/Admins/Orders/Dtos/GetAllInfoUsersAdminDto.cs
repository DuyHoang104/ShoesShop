using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Web.Modules.Admins.Users.Dtos;

namespace ShoesShop.Web.Modules.Admins.Orders.Dtos;

public class GetAllInfoUsersAdminDto
{
    public int ID { get; set; }
    public string UserName { get; set; }

    public string Password { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public char LastAction { get; set; }

    public UserGender Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public List<AddressAdminDto> Addresses { get; set; }

    public UserStatus Status { get; set; }

    public UserAccountRole Role { get; set; }

    public List<OrderAdminDto> Orders { get; set; }
}