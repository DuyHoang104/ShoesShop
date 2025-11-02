using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Domain.Modules.Users.Dtos.Commands
{
    public class CheckEmailCommandDto
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Incorrect Email Format")]
        public string Email { get; set; }
    }
}
