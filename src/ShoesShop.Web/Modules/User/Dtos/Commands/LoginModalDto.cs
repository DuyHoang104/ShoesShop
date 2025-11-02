using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Web.Modules.User.Dtos.Commands
{
    public class LoginModalDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "UserName")]
        [MaxLength(20, ErrorMessage = "Max length is 20 characters!")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Username must not contain spaces.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Password must not contain spaces.")]
        public required string Password { get; set; }
    }
}