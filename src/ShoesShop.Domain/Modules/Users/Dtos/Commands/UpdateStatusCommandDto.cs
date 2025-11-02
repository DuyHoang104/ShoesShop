using ShoesShop.Domain.Modules.Users.Enums;

namespace ShoesShop.Domain.Modules.Users.Dtos.Commands
{
    public class UpdateStatusCommandDto
    {
        public string Email { get; set; }

        public UserStatus Status { get; set; }
    }
}
