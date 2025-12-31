using System.ComponentModel.DataAnnotations;

namespace ShoesShop.Domain.Users.Dtos.Commands;

public class CheckEmailCommand
{
    [Required(ErrorMessage = "*")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Incorrect Email Format")]
    public string Email { get; set; }
}
