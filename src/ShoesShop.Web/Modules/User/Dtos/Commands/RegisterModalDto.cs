using System.ComponentModel.DataAnnotations;
using ShoesShop.Domain.Modules.Users.Dtos;
using ShoesShop.Domain.Modules.Users.Enums;

namespace ShoesShop.Web.Modules.User.Dtos.Commands
{
    public class RegisterModalDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "UserName")]
        [MaxLength(20, ErrorMessage = "Max length is 20 characters!")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username must not contain spaces.")]
        public required string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain spaces.")]
        public required string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public required string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Date Of Birth")]
        [Required(ErrorMessage = "Date Of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Incorrect Email Format.")]
        [Display(Name = "Email")]
        public required string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(24, ErrorMessage = "Max length is 24 characters!")]
        [RegularExpression(@"^0[9875]\d{8}$", ErrorMessage = "Incorrect phone format.")]
        [Display(Name = "Phone")]
        public required string Phone { get; set; } = string.Empty;

        public UserGender Gender { get; set; }
        

        [Display(Name = "Avatar")]
        public IFormFile? AvatarUrl { get; set; }

        public List<AddressDto> Addresses { get; set; } = new();
    }
}