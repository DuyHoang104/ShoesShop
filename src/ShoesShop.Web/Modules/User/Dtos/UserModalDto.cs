using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Enums;

namespace ShoesShop.Web.Modules.User.Dtos
{
    public class UserModalDto
    {
        public required string UserName { get; set; }

        public required string Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public char? LastAction { get; set; }

        public UserStatus? Status { get; set; }

        public UserGender? Gender { get; set; }

        public string? AvatarUrl { get; set; }

        public virtual Address? Address { get; set; }
    }
}