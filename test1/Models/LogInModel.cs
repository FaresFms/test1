using System.ComponentModel.DataAnnotations;

namespace test1.Models
{
    public class LogInModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Please enter the Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [StringLength(20, ErrorMessage = "the password must be between 20 and 10", MinimumLength = 10)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "The Password must contains special character , numbers and capital letter ")]
        public string Password { get; set; }
    }
}
