

using ShoesShop.Domain.Modules.User.Users.Enums;

namespace ShoesShop.Domain.Modules.User.Users.Dtos.Commands;

public class UpdateStatusCommandDto
{
    public string Email { get; set; }

    public UserStatus Status { get; set; }
}

