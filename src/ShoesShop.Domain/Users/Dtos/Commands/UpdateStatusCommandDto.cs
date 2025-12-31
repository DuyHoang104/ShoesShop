using ShoesShop.Domain.Users.Enums;

namespace ShoesShop.Domain.Users.Dtos.Commands;

public class UpdateStatusCommandDto
{
    public string Email { get; set; }

    public UserStatus Status { get; set; }
}