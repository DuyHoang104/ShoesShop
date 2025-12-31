using ShoesShop.Domain.Users.Enums;
using ShoesShop.Web.User.Dtos.Commands.Admin;

namespace ShoesShop.Web.Order.Dtos.Admin.Commands;

public class GetAllInfoUsersDto
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

    public List<AddressDto> Addresses { get; set; }

    public UserStatus Status { get; set; }

    public UserAccountRole Role { get; set; }

    public List<OrderDto> Orders { get; set; }
}