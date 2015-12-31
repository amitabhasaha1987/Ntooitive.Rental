using System.ComponentModel.DataAnnotations;
namespace Repositories.Models.ViewModel
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter email id")]
        [Display(Name = "E-Mail ")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct email id")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}