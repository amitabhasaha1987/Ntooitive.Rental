using System.ComponentModel.DataAnnotations;

namespace Repositories.Models.ViewModel
{
    public class RegistartionViewModel
    {
        [Required(ErrorMessage = "Please enter first name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [Display(Name = "E-Mail ")]
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please enter correct email id")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password and this field should be identical")]
        [Compare("Password")]
        public string ConfirmPasssword { get; set; }
        public string ParticipantId { get; set; }

    }
}