using Microsoft.AspNetCore.Http;
using ShoesShop.Domain.Modules.User.Users.Enums;

namespace ShoesShop.Domain.Modules.User.Users.Dtos.Commands
{
    public class RegisterCommandDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public UserGender Gender { get; set; }

        public IFormFile? AvatarUrl { get; set; }

        public List<AddressDto> Addresses { get; set; } = new();

        public UserAccountRole Role { get; set; }
    }
}